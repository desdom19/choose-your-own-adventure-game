public enum FinalFightOutcome
{
    Won,
    Lost,
    Retreated,
}

// The Final Fight at the end of the north and west paths, from the Monster's path narrative
// to the ending text. Every outcome ends the adventure.
public static class FinalFight
{
    // Every roll in the fight comes from the die passed in: both sides' attack, defense and
    // damage rolls, and the Monster's taunt and reply picks.
    public static FinalFightOutcome Run(Player player, Monster monster, Messages messages, Die die)
    {
        ShowEncounter(player, monster, messages);

        if (!PlayerChoosesToAttack(messages))
        {
            Console.WriteLine(messages.GetMessage("retreat"));
            return FinalFightOutcome.Retreated;
        }

        FinalFightOutcome outcome = PlayRounds(player, monster, messages, die);

        if (outcome == FinalFightOutcome.Won)
            Console.WriteLine(messages.GetMessage(monster.VictoryKey));
        else if (outcome == FinalFightOutcome.Lost)
            Console.WriteLine(messages.GetMessage(monster.DefeatKey));

        return outcome;
    }

    // The Monster's path narrative, its intro in red, then its stats.
    private static void ShowEncounter(Player player, Monster monster, Messages messages)
    {
        Console.WriteLine(messages.GetMessage(monster.PathNarrativeKey));

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(string.Format(messages.GetMessage(monster.IntroKey), monster.Name));
        Console.ResetColor();

        Console.WriteLine(messages.GetMessage(monster.StatsIntroKey));
        ShowMonsterStats(monster, messages);
    }

    // The attack-or-retreat prompt before the first Round. Asks again on an invalid answer.
    private static bool PlayerChoosesToAttack(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("encounter_prompt"));
            Console.Write(messages.GetMessage("enter_choice"));
            string input = ConsoleInput.ReadLine().ToLower();

            if (input == "a" || input == "attack")
                return true;

            if (input == "r" || input == "retreat")
                return false;

            Console.WriteLine(messages.GetMessage("invalid"));
        }
    }

    // Each Round: the player's Strike, the Monster's Strike, then the choice to attack again
    // or retreat. A retreat here shows its own text, and no ending follows it.
    private static FinalFightOutcome PlayRounds(Player player, Monster monster, Messages messages, Die die)
    {
        Console.WriteLine(GetStatsDisplay(player, monster, messages));

        while (true)
        {
            Console.WriteLine(PlayerStrike(player, monster, messages, die));

            if (monster.HealthPoints <= 0)
                return FinalFightOutcome.Won;

            Console.WriteLine(MonsterStrike(monster, player, messages, die));

            if (player.HealthPoints <= 0)
                return FinalFightOutcome.Lost;

            if (!PlayerAttacksAgain(messages))
            {
                Console.WriteLine(messages.GetMessage("retreat_combat"));
                return FinalFightOutcome.Retreated;
            }
        }
    }

    // The player's Strike against the Monster. The Monster dodges with a taunt that can name
    // the player's race, and replies to damage unless it is defeated.
    private static string PlayerStrike(Player player, Monster monster, Messages messages, Die die)
    {
        if (player.Weapon == null)
            return messages.GetMessage("player_no_weapon");

        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage(monster.PlayerAttackIntroKey),
            messages.TranslateOccupationForDisplay(player.Occupation),
            messages.TranslateWeaponForDisplay(player.Weapon.Type)
        ));

        output.Add(player.Weapon.AsciiArt);

        int attackRoll = RollAttack(die);

        // MISS
        if (!AttackHits(attackRoll, player.Strength))
        {
            output.Add(string.Format(messages.GetMessage("player_missed_monster"), monster.Name));
            return string.Join(Environment.NewLine, output);
        }

        output.Add(string.Format(messages.GetMessage("player_hit_monster"), monster.Name));

        int defenseRoll = die.Roll(20);

        // DEFENSE: a roll of 20 never dodges, so a Monster with Agility 20 can still be hurt.
        if (defenseRoll < 20 && defenseRoll <= monster.Agility)
        {
            output.Add(string.Format(messages.GetMessage("monster_defended_player_attack"), monster.Name));
            output.Add(string.Format(
                PickDefenseTaunt(monster, messages, die),
                messages.TranslateRaceForDisplay(player.Race)
            ));
            return string.Join(Environment.NewLine, output);
        }

        // DAMAGE
        int damage = RollPlayerDamage(player, die);
        monster.HealthPoints -= damage;

        output.Add(string.Format(messages.GetMessage("player_damage_dealt"), damage));
        output.Add(string.Format(messages.GetMessage("monster_health_now"), monster.Name, monster.HealthPoints));

        // END
        if (monster.HealthPoints <= 0)
            output.Add(messages.GetMessage(monster.DefeatedNarrativeKey));
        else
            output.Add(PickDamageReply(monster, messages, die));

        return string.Join(Environment.NewLine, output);
    }

    // The Monster's Strike against the player. The player's Armor absorbs part of a hit, and
    // the player's Health Points stop at 0.
    private static string MonsterStrike(Monster monster, Player player, Messages messages, Die die)
    {
        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage("monster_attack_intro"),
            monster.Name,
            player.Name,
            messages.TranslateWeaponForDisplay(monster.Weapon.Type)
        ));

        if (!string.IsNullOrWhiteSpace(monster.Weapon.AsciiArt))
            output.Add(monster.Weapon.AsciiArt);

        int attackRoll = RollAttack(die);

        if (!AttackHits(attackRoll, monster.Strength))
        {
            output.Add(string.Format(messages.GetMessage("monster_missed_player"), monster.Name, player.Name));
            return string.Join(Environment.NewLine, output);
        }

        output.Add(string.Format(messages.GetMessage("monster_hit_player"), monster.Name, player.Name));

        int defenseRoll = die.Roll(20);

        // A roll of 20 never dodges, so a player with Agility 20 or more can still be hurt.
        if (defenseRoll < 20 && defenseRoll <= player.Agility)
        {
            output.Add(string.Format(messages.GetMessage("player_defended_monster_attack"), player.Name));
            return string.Join(Environment.NewLine, output);
        }

        int damageRolled = RollMonsterDamage(monster, die);
        int damageTaken = Math.Max(0, damageRolled - player.Protection);
        player.HealthPoints -= damageTaken;

        if (player.HealthPoints < 0)
            player.HealthPoints = 0;

        output.Add(string.Format(messages.GetMessage("monster_damage_dealt"), monster.Name, damageRolled));

        if (player.Armor != null)
            output.Add(DescribeArmorAbsorption(player.Armor, damageRolled - damageTaken, damageTaken, messages));

        output.Add(string.Format(messages.GetMessage("player_health_now"), player.Name, player.HealthPoints));

        if (player.HealthPoints <= 0)
            output.Add(string.Format(messages.GetMessage("player_defeated_narrative"), player.Name));

        return string.Join(Environment.NewLine, output);
    }

    private static int RollAttack(Die die)
    {
        return die.Roll(20);
    }

    private static bool AttackHits(int attackRoll, int strength)
    {
        return attackRoll <= strength;
    }

    private static int RollPlayerDamage(Player player, Die die)
    {
        if (player.Weapon == null)
            return 0;

        return die.Roll(player.Weapon.MaxDamage);
    }

    private static int RollMonsterDamage(Monster monster, Die die)
    {
        return die.Roll(monster.Weapon.MaxDamage);
    }

    private static string DescribeArmorAbsorption(Armor armor, int damageAbsorbed, int damageTaken, Messages messages)
    {
        string armorName = messages.TranslateArmorForDisplay(armor.Name);

        if (damageTaken == 0)
            return string.Format(messages.GetMessage("armor_absorbed_whole_hit"), armorName);

        return string.Format(messages.GetMessage("armor_absorbed"), armorName, damageAbsorbed, damageTaken);
    }

    // A defense taunt can contain {0}, filled with the player's race.
    private static string PickDefenseTaunt(Monster monster, Messages messages, Die die)
    {
        int index = die.Roll(monster.DefenseTauntKeys.Count) - 1;
        return messages.GetMessage(monster.DefenseTauntKeys[index]);
    }

    private static string PickDamageReply(Monster monster, Messages messages, Die die)
    {
        int index = die.Roll(monster.DamageReplyKeys.Count) - 1;
        return messages.GetMessage(monster.DamageReplyKeys[index]);
    }

    // The choice to attack again or retreat after a Round. Asks again on an invalid answer.
    private static bool PlayerAttacksAgain(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("attack_prompt"));
            string input = ConsoleInput.ReadLine().ToLower();

            if (input == "a" || input == "attack")
                return true;

            if (input == "r" || input == "retreat")
                return false;

            Console.WriteLine(messages.GetMessage("invalid"));
        }
    }

    // The Monster's own stats, shown when the player meets it.
    private static void ShowMonsterStats(Monster monster, Messages messages)
    {
        Console.WriteLine(messages.GetMessage(monster.StatsHeaderKey));
        Console.WriteLine(messages.GetMessage("stats_name") + monster.Name);
        Console.WriteLine(messages.GetMessage("stats_strength") + monster.Strength);
        Console.WriteLine(messages.GetMessage("stats_agility") + monster.Agility);
        Console.WriteLine(messages.GetMessage("stats_health") + monster.HealthPoints);
        Console.WriteLine(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(monster.Weapon.Type));
        Console.WriteLine(messages.GetMessage("stats_damage") + monster.Weapon.MaxDamage);
    }

    // Both sides' stats, shown once before the first Round.
    private static string GetStatsDisplay(Player player, Monster monster, Messages messages)
    {
        var output = new List<string>();

        output.Add(messages.GetMessage("combat_stats_header"));
        output.Add(messages.GetMessage("combat_player_header"));
        output.Add(messages.GetMessage("stats_name") + player.Name);
        output.Add(messages.GetMessage("stats_strength") + player.Strength);
        output.Add(messages.GetMessage("stats_agility") + player.Agility);
        output.Add(messages.GetMessage("stats_health") + player.HealthPoints);
        output.Add(messages.GetMessage("stats_protection") + player.Protection);

        if (player.Weapon != null)
        {
            output.Add(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(player.Weapon.Type));
            output.Add(messages.GetMessage("stats_damage") + player.Weapon.MaxDamage);
        }

        output.Add("");
        output.Add(messages.GetMessage(monster.CombatHeaderKey));
        output.Add(messages.GetMessage("stats_name") + monster.Name);
        output.Add(messages.GetMessage("stats_strength") + monster.Strength);
        output.Add(messages.GetMessage("stats_agility") + monster.Agility);
        output.Add(messages.GetMessage("stats_health") + monster.HealthPoints);
        output.Add(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(monster.Weapon.Type));
        output.Add(messages.GetMessage("stats_damage") + monster.Weapon.MaxDamage);

        return string.Join(Environment.NewLine, output);
    }
}
