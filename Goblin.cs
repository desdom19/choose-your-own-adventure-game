// The Monster met on the west path. It's slippery, so it dodges more often than the
// Dragon (on Normal its Agility is never below 6; on Easy it can be as low as 3), but its
// rusty cleaver hurts less than claws.
public class Goblin : Monster
{
    private const int AgilityBase = 5;
    private const int AgilityDieSides = 15;

    private static readonly IReadOnlyList<string> _defenseTauntKeys = new[]
    {
        "goblin_defense_taunt_1",
        "goblin_defense_taunt_2",
        "goblin_defense_taunt_3",
        "goblin_defense_taunt_4"
    };

    private static readonly IReadOnlyList<string> _damageReplyKeys = new[]
    {
        "goblin_damage_reply_1",
        "goblin_damage_reply_2",
        "goblin_damage_reply_3",
        "goblin_damage_reply_4"
    };

    public Goblin(Die die, Difficulty? difficulty = null)
        : base(die, "Gritknuckle", new Weapon("rusty cleaver", 9, "|=[####]"), difficulty, RollStats)
    {
    }

    private static RolledStats RollStats(Die die)
    {
        int strength = die.Roll(20);
        int agility = AgilityBase + die.Roll(AgilityDieSides);
        int healthPoints = die.Roll(20);
        return new RolledStats(strength, agility, healthPoints);
    }

    public override string PathNarrativeKey => "goblin_path_narrative";
    public override string IntroKey => "goblin_intro";
    public override string StatsIntroKey => "goblin_stats_intro";
    public override string StatsHeaderKey => "goblin_stats_header";
    public override string CombatHeaderKey => "goblin_combat_header";
    public override string PlayerAttackIntroKey => "goblin_player_attack_intro";
    public override string DefeatedNarrativeKey => "goblin_defeated_narrative";
    public override string VictoryKey => "goblin_victory";
    public override string DefeatKey => "goblin_defeat";

    public override IReadOnlyList<string> DefenseTauntKeys => _defenseTauntKeys;
    public override IReadOnlyList<string> DamageReplyKeys => _damageReplyKeys;
}
