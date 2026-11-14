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
    // Each side's Strike still rolls on that side's own Die, so the die passed in is not rolled yet.
    public static FinalFightOutcome Run(Player player, Monster monster, Messages messages, Die die)
    {
        ShowEncounter(player, monster, messages);

        if (!PlayerChoosesToAttack(messages))
        {
            Console.WriteLine(messages.GetMessage("retreat"));
            return FinalFightOutcome.Retreated;
        }

        FinalFightOutcome outcome = PlayRounds(player, monster, messages);

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
        monster.DisplayStats(messages);
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
    private static FinalFightOutcome PlayRounds(Player player, Monster monster, Messages messages)
    {
        Console.WriteLine(GetStatsDisplay(player, monster, messages));

        while (true)
        {
            Console.WriteLine(player.Attack(monster, messages));

            if (monster.HealthPoints <= 0)
                return FinalFightOutcome.Won;

            Console.WriteLine(monster.Attack(player, messages));

            if (player.HealthPoints <= 0)
                return FinalFightOutcome.Lost;

            if (!PlayerAttacksAgain(messages))
            {
                Console.WriteLine(messages.GetMessage("retreat_combat"));
                return FinalFightOutcome.Retreated;
            }
        }
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
