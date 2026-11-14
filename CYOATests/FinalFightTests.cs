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

    private static Player CreatePlayer(Die die, int healthPoints)
    {
        return new Player(die)
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

    // Strength 10, Agility 5 + 5, Health Points 6, then the given fight rolls.
    private static Goblin CreateGoblin(params int[] fightRolls)
    {
        return new Goblin(new ScriptedDie(new[] { 10, 5, 6 }.Concat(fightRolls).ToArray()));
    }

    private static FinalFightOutcome Run(string input, Player player, Monster monster, out string output)
    {
        Console.SetIn(new StringReader(input));
        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        FinalFightOutcome outcome = FinalFight.Run(player, monster, GetMessages(), new ScriptedDie());

        output = writer.ToString().Replace("\r\n", "\n").TrimEnd();
        return outcome;
    }

    [TestMethod]
    public void RetreatAtTheEncounterPrompt_ReturnsRetreated_WithNoDiceRolled()
    {
        // Every Die is empty after the Goblin's stats, so any roll would throw.
        FinalFightOutcome outcome = Run("r\n", CreatePlayer(new ScriptedDie(), 20), CreateGoblin(), out string output);

        Assert.AreEqual(FinalFightOutcome.Retreated, outcome);
        StringAssert.EndsWith(output, "You quickly retreat. You come home poor but alive to adventure another day. The End.");
    }

    [TestMethod]
    public void PlayerBringsTheMonsterToZero_ReturnsWon_WithTheVictoryEnding()
    {
        // The player hits (1), the Goblin fails to dodge (20) and takes 6, all its Health Points.
        Goblin goblin = CreateGoblin();

        FinalFightOutcome outcome = Run("a\n", CreatePlayer(new ScriptedDie(1, 20, 6), 20), goblin, out string output);

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
        Player player = CreatePlayer(new ScriptedDie(20), 3);

        FinalFightOutcome outcome = Run("a\n", player, CreateGoblin(1, 20, 9), out string output);

        Assert.AreEqual(FinalFightOutcome.Lost, outcome);
        Assert.AreEqual(0, player.HealthPoints);
        StringAssert.EndsWith(output, string.Join("\n",
            "TestHero has been defeated.",
            "The goblin cackles and rummages through your pockets as your eyesight fades. You are defeated. Your family mourns their loss. The End."));
    }

    [TestMethod]
    public void StatsBeforeTheFirstRound_ShowBothSides_WithProtectionZeroWithoutArmor()
    {
        FinalFightOutcome outcome = Run("a\nr\n", CreatePlayer(new ScriptedDie(20), 20), CreateGoblin(15), out string output);

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
        FinalFightOutcome outcome = Run("a\nr\n", CreatePlayer(new ScriptedDie(20), 20), CreateGoblin(15), out string output);

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
        FinalFightOutcome outcome = Run("x\nr\n", CreatePlayer(new ScriptedDie(), 20), CreateGoblin(), out string output);

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
        Goblin goblin = CreateGoblin(15);
        Player player = CreatePlayer(new ScriptedDie(20), 20);

        FinalFightOutcome outcome = Run("a\nx\nr\n", player, goblin, out string output);

        Assert.AreEqual(FinalFightOutcome.Retreated, outcome);
        StringAssert.EndsWith(output, string.Join("\n",
            "Gritknuckle missed TestHero.",
            "Enter 'a' to attack or 'r' to retreat.",
            "Invalid choice. Please try again.",
            "Enter 'a' to attack or 'r' to retreat.",
            "You hastily retreat licking your wounds."));
    }
}
