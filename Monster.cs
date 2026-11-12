// What the player meets at the end of a path and fights in a Final Fight.
// Combat behavior that is the same for every Monster lives here. Each Monster supplies
// its name, how its stats are rolled, its weapon and the message keys for its own text.
public abstract class Monster
{
    protected readonly Die _die;

    public string Name { get; set; }
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int HealthPoints { get; set; }

    public Weapon Weapon { get; set; }

    protected Monster(Die die, string name, Weapon weapon)
    {
        _die = die;
        Name = name;
        Weapon = weapon;
    }

    // Message keys for this Monster's own text, looked up in the player's language.
    public abstract string PathNarrativeKey { get; }
    public abstract string IntroKey { get; }
    public abstract string StatsIntroKey { get; }
    public abstract string StatsHeaderKey { get; }
    public abstract string CombatHeaderKey { get; }
    public abstract string PlayerAttackIntroKey { get; }
    public abstract string DefeatedNarrativeKey { get; }
    public abstract string VictoryKey { get; }
    public abstract string DefeatKey { get; }

    // A defense taunt can contain {0}, filled with the player's race (see Player.Attack).
    protected abstract IReadOnlyList<string> DefenseTauntKeys { get; }
    protected abstract IReadOnlyList<string> DamageReplyKeys { get; }

    public int RollAttack()
    {
        return _die.Roll(20);
    }

    public bool AttackHits(int attackRoll)
    {
        return attackRoll <= Strength;
    }

    public int RollDamage()
    {
        return _die.Roll(Weapon.MaxDamage);
    }

    public string Attack(Player player, Messages messages)
    {
        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage("monster_attack_intro"),
            Name,
            player.Name,
            messages.TranslateWeaponForDisplay(Weapon.Type)
        ));

        if (!string.IsNullOrWhiteSpace(Weapon.AsciiArt))
            output.Add(Weapon.AsciiArt);

        int attackRoll = RollAttack();

        if (!AttackHits(attackRoll))
        {
            output.Add(string.Format(messages.GetMessage("monster_missed_player"), Name, player.Name));
            return string.Join(Environment.NewLine, output);
        }

        output.Add(string.Format(messages.GetMessage("monster_hit_player"), Name, player.Name));

        int defenseRoll = _die.Roll(20);

        if (defenseRoll <= player.Agility)
        {
            output.Add(string.Format(messages.GetMessage("player_defended_monster_attack"), player.Name));
            return string.Join(Environment.NewLine, output);
        }

        int damageRolled = RollDamage();
        int damageTaken = Math.Max(0, damageRolled - player.Protection);
        player.HealthPoints -= damageTaken;

        if (player.HealthPoints < 0)
            player.HealthPoints = 0;

        output.Add(string.Format(messages.GetMessage("monster_damage_dealt"), Name, damageRolled));

        if (player.Armor != null)
            output.Add(DescribeArmorAbsorption(player.Armor, damageRolled - damageTaken, damageTaken, messages));

        output.Add(string.Format(messages.GetMessage("player_health_now"), player.Name, player.HealthPoints));

        if (player.HealthPoints <= 0)
            output.Add(string.Format(messages.GetMessage("player_defeated_narrative"), player.Name));

        return string.Join(Environment.NewLine, output);
    }

    private static string DescribeArmorAbsorption(Armor armor, int damageAbsorbed, int damageTaken, Messages messages)
    {
        string armorName = messages.TranslateArmorForDisplay(armor.Name);

        if (damageTaken == 0)
            return string.Format(messages.GetMessage("armor_absorbed_whole_hit"), armorName);

        return string.Format(messages.GetMessage("armor_absorbed"), armorName, damageAbsorbed, damageTaken);
    }

    public string GetRandomDefenseTaunt(Messages messages)
    {
        int index = _die.Roll(DefenseTauntKeys.Count) - 1;
        return messages.GetMessage(DefenseTauntKeys[index]);
    }

    public string GetRandomDamageReply(Messages messages)
    {
        int index = _die.Roll(DamageReplyKeys.Count) - 1;
        return messages.GetMessage(DamageReplyKeys[index]);
    }

    public void DisplayStats(Messages messages)
    {
        Console.WriteLine(messages.GetMessage(StatsHeaderKey));
        Console.WriteLine(messages.GetMessage("stats_name") + Name);
        Console.WriteLine(messages.GetMessage("stats_strength") + Strength);
        Console.WriteLine(messages.GetMessage("stats_agility") + Agility);
        Console.WriteLine(messages.GetMessage("stats_health") + HealthPoints);
        Console.WriteLine(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(Weapon.Type));
        Console.WriteLine(messages.GetMessage("stats_damage") + Weapon.MaxDamage);
    }
}
