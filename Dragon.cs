// The Monster met on the north path.
public class Dragon : Monster
{
    private static readonly IReadOnlyList<string> _defenseTauntKeys = new[]
    {
        "dragon_defense_taunt_1",
        "dragon_defense_taunt_2",
        "dragon_defense_taunt_3",
        "dragon_defense_taunt_4"
    };

    private static readonly IReadOnlyList<string> _damageReplyKeys = new[]
    {
        "dragon_damage_reply_1",
        "dragon_damage_reply_2",
        "dragon_damage_reply_3",
        "dragon_damage_reply_4"
    };

    public Dragon(Die die, Difficulty? difficulty = null)
        : base(die, "Smolderfang", new Weapon("claws", 12, "<<< claws >>>"), difficulty, RollStats)
    {
    }

    private static RolledStats RollStats(Die die)
    {
        int strength = die.Roll(20);
        int agility = die.Roll(20);
        int healthPoints = die.Roll(20);
        return new RolledStats(strength, agility, healthPoints);
    }

    public override string PathNarrativeKey => "dragon_path_narrative";
    public override string IntroKey => "dragon_intro";
    public override string StatsIntroKey => "dragon_stats_intro";
    public override string StatsHeaderKey => "dragon_stats_header";
    public override string CombatHeaderKey => "dragon_combat_header";
    public override string PlayerAttackIntroKey => "dragon_player_attack_intro";
    public override string DefeatedNarrativeKey => "dragon_defeated_narrative";
    public override string VictoryKey => "dragon_victory";
    public override string DefeatKey => "dragon_defeat";

    protected override IReadOnlyList<string> DefenseTauntKeys => _defenseTauntKeys;
    protected override IReadOnlyList<string> DamageReplyKeys => _damageReplyKeys;
}
