// What the player meets at the end of a path and fights in a Final Fight.
// Each Monster supplies its name, how its stats are rolled, its weapon and the message keys
// for its own text. How it fights lives in the Final Fight.
public abstract class Monster
{
    public string Name { get; set; }
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int HealthPoints { get; set; }

    public Weapon Weapon { get; set; }

    // The stats a Monster rolls when it's created, before Difficulty changes them.
    protected record RolledStats(int Strength, int Agility, int HealthPoints);

    // The Monster rolls its stats, then the Difficulty changes them and the maximum damage
    // of its own weapon. A Monster built without a Difficulty gets Normal.
    protected Monster(Die die, string name, Weapon weapon, Difficulty? difficulty, Func<Die, RolledStats> rollStats)
    {
        Name = name;
        Weapon = weapon;

        difficulty ??= Difficulty.Normal;
        RolledStats rolled = rollStats(die);

        Strength = difficulty.ChangeStrength(rolled.Strength);
        Agility = difficulty.ChangeAgility(rolled.Agility);
        HealthPoints = difficulty.ChangeHealthPoints(rolled.HealthPoints);
        Weapon.MaxDamage = difficulty.ChangeWeaponMaxDamage(Weapon.MaxDamage);
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

    // A defense taunt can contain {0}, filled with the player's race in the Final Fight.
    public abstract IReadOnlyList<string> DefenseTauntKeys { get; }
    public abstract IReadOnlyList<string> DamageReplyKeys { get; }
}
