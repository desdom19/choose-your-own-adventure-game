using System.IO;

// What a Final Fight returns and the choices it asks for, through FinalFight.Run.
[TestClass]
[DoNotParallelize]
public class FinalFightTests
{
    private TextReader? _originalIn;
    private TextWriter? _originalOut;

    [TestInitialize]
    public void Setup()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_originalIn != null)
            Console.SetIn(_originalIn);

        if (_originalOut != null)
            Console.SetOut(_originalOut);
    }

    private static Messages GetMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        return messages;
    }

    // The player's own Die is empty: every roll in a Final Fight comes from the Die given to Run.
    private static Player CreatePlayer(int healthPoints)
    {
        return new Player(new ScriptedDie())
        {
            Name = "TestHero",
            Race = "Elf",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = healthPoints,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    // Strength 10, Agility 5 + 5, Health Points 6. Its Die has no rolls left for the fight.
    private static Goblin CreateGoblin()
    {
        return new Goblin(new ScriptedDie(10, 5, 6));
    }

    // Runs a Final Fight whose rolls, in fight order, all come from the one Die given to Run.
    private static FinalFightOutcome Run(string input, Player player, Monster monster, int[] rolls, out string output)
    {
        Console.SetIn(new StringReader(input));
        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        FinalFightOutcome outcome = FinalFight.Run(player, monster, GetMessages(), new ScriptedDie(rolls));

        output = writer.ToString().Replace("\r\n", "\n").TrimEnd();
        return outcome;
    }

    [TestMethod]
    public void RetreatAtTheEncounterPrompt_ReturnsRetreated_WithNoDiceRolled()
    {
        // The Die is empty, so any roll would throw.
        FinalFightOutcome outcome = Run("r\n", CreatePlayer(20), CreateGoblin(), [], out string output);

        Assert.AreEqual(FinalFightOutcome.Retreated, outcome);
        StringAssert.EndsWith(output, "You quickly retreat. You come home poor but alive to adventure another day. The End.");
    }

    [TestMethod]
    public void PlayerBringsTheMonsterToZero_ReturnsWon_WithTheVictoryEnding()
    {
        // The player hits (1), the Goblin fails to dodge (20) and takes 6, all its Health Points.
        Goblin goblin = CreateGoblin();

        FinalFightOutcome outcome = Run("a\n", CreatePlayer(20), goblin, [1, 20, 6], out string output);

        Assert.AreEqual(FinalFightOutcome.Won, outcome);
        Assert.AreEqual(0, goblin.HealthPoints);
        StringAssert.EndsWith(output, string.Join("\n",
            "With a last squeal of 'Eeek!', you have defeated the goblin.",
            "With the goblin beaten, the west road is safe again. You return home a hero. The End."));
    }

    [TestMethod]
    public void MonsterBringsThePlayerToZero_ReturnsLost_WithTheDefeatEnding()
    {
        // The player misses (20); the Goblin hits (1), the dodge fails (20) and it deals 9.
        Player player = CreatePlayer(3);

        FinalFightOutcome outcome = Run("a\n", player, CreateGoblin(), [20, 1, 20, 9], out string output);

        Assert.AreEqual(FinalFightOutcome.Lost, outcome);
        Assert.AreEqual(0, player.HealthPoints);
        StringAssert.EndsWith(output, string.Join("\n",
            "TestHero has been defeated.",
            "The goblin cackles and rummages through your pockets as your eyesight fades. You are defeated. Your family mourns their loss. The End."));
    }

    [TestMethod]
    public void StatsBeforeTheFirstRound_ShowBothSides_WithProtectionZeroWithoutArmor()
    {
        FinalFightOutcome outcome = Run("a\nr\n", CreatePlayer(20), CreateGoblin(), [20, 15], out string output);

        Assert.AreEqual(FinalFightOutcome.Retreated, outcome);
        StringAssert.Contains(output, string.Join("\n",
            "Enter choice: ===== Combat Stats =====",
            "--- Player ---",
            "Name: TestHero",
            "Strength: 15",
            "Agility: 12",
            "Health Points: 20",
            "Protection: 0",
            "Weapon: long sword",
            "Weapon's Maximum Damage Points: 12",
            "",
            "--- Goblin ---",
            "Name: Gritknuckle",
            "Strength: 10",
            "Agility: 10",
            "Health Points: 6",
            "Weapon: rusty cleaver",
            "Weapon's Maximum Damage Points: 9",
            "As a Fighter, you withdraw your long sword and attack the goblin."));
    }

    [TestMethod]
    public void RetreatBetweenRounds_ReturnsRetreated_WithTheMidFightRetreatTextOnly()
    {
        // One Round where both sides miss (20 and 15), then a retreat.
        FinalFightOutcome outcome = Run("a\nr\n", CreatePlayer(20), CreateGoblin(), [20, 15], out string output);

        Assert.AreEqual(FinalFightOutcome.Retreated, outcome);
        StringAssert.EndsWith(output, string.Join("\n",
            "Gritknuckle missed TestHero.",
            "Enter 'a' to attack or 'r' to retreat.",
            "You hastily retreat licking your wounds."));
        Assert.IsFalse(output.Contains("You quickly retreat."), "A retreat between Rounds should not show the encounter retreat text.");
    }

    [TestMethod]
    public void InvalidAnswerAtTheEncounterPrompt_AsksAgain()
    {
        FinalFightOutcome outcome = Run("x\nr\n", CreatePlayer(20), CreateGoblin(), [], out string output);

        Assert.AreEqual(FinalFightOutcome.Retreated, outcome);
        StringAssert.EndsWith(output, string.Join("\n",
            "Do you wish to retreat or attack? Enter 'r' for retreat. Enter 'a' for attack.",
            "Enter choice: Invalid choice. Please try again.",
            "Do you wish to retreat or attack? Enter 'r' for retreat. Enter 'a' for attack.",
            "Enter choice: You quickly retreat. You come home poor but alive to adventure another day. The End."));
    }

    [TestMethod]
    public void InvalidAnswerBetweenRounds_AsksAgainWithoutPlayingARound()
    {
        // One Round where both sides miss (20 and 15), then "x", then a retreat.
        FinalFightOutcome outcome = Run("a\nx\nr\n", CreatePlayer(20), CreateGoblin(), [20, 15], out string output);

        Assert.AreEqual(FinalFightOutcome.Retreated, outcome);
        StringAssert.EndsWith(output, string.Join("\n",
            "Gritknuckle missed TestHero.",
            "Enter 'a' to attack or 'r' to retreat.",
            "Invalid choice. Please try again.",
            "Enter 'a' to attack or 'r' to retreat.",
            "You hastily retreat licking your wounds."));
    }

    [TestMethod]
    public void EveryRoll_ComesFromTheDieGivenToRun_InFightOrder()
    {
        // Round 1: the player hits (1), the Goblin dodges (5) and picks taunt 3; the Goblin
        // hits (1), the dodge fails (20) and it deals 4. Round 2: the player hits (1), no dodge
        // (20), deals 2, and the Goblin picks reply 2; the Goblin misses (15). Then a retreat.
        Goblin goblin = CreateGoblin();
        Player player = CreatePlayer(20);

        FinalFightOutcome outcome = Run("a\na\nr\n", player, goblin, [1, 5, 3, 1, 20, 4, 1, 20, 2, 2, 15], out string output);

        Assert.AreEqual(FinalFightOutcome.Retreated, outcome);
        Assert.AreEqual(16, player.HealthPoints);
        Assert.AreEqual(4, goblin.HealthPoints);
        StringAssert.Contains(output, "Over here, Elf! No, over here! Hee hee!");
        StringAssert.Contains(output, "You'll pay for that, you big lump!");
    }
}
