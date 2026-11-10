using System.IO;

[TestClass]
[DoNotParallelize]
public class CharacterCreationTests
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

    [TestMethod]
    [DataRow("fighter", "long sword", 12, "-)=====>")]
    [DataRow("magician", "lightning bolt spell", 12, "zap~~~~~~")]
    [DataRow("thief", "dagger", 6, "-)==>")]
    [DataRow("archer", "long bow", 8, "}    -->")]
    public void CreateCharacter_GivesEachOccupationItsStartingWeapon(
        string occupation, string weaponType, int maxDamage, string asciiArt)
    {
        Messages messages = CreateMessages();

        // Human, so no extra Agility roll: Strength d20 + d4 modifier, Agility d20, Health d20, Gold d20.
        Player player = new Player(new ScriptedDie(10, 2, 10, 10, 10));

        Console.SetIn(new StringReader($"human\nTestHero\n{occupation}\nroll\nroll\nroll\nroll\n2\n"));
        Console.SetOut(new StringWriter());

        player.CreateCharacter(messages);

        Assert.IsNotNull(player.Weapon);
        Assert.AreEqual(weaponType, player.Weapon.Type);
        Assert.AreEqual(maxDamage, player.Weapon.MaxDamage);
        Assert.AreEqual(asciiArt, player.Weapon.AsciiArt);
    }

    private static Messages CreateMessages(string language = "English")
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    [TestMethod]
    public void NewPlayer_HasNoGold()
    {
        Assert.AreEqual(0, new Player().Gold);
    }

    [TestMethod]
    [DataRow(1, 10)]
    [DataRow(7, 70)]
    [DataRow(20, 200)]
    public void CreateCharacter_GoldIsTheD20RollTimesTen(int goldRoll, int expectedGold)
    {
        // Human: Strength d20 + d4, Agility d20, Health d20, Gold d20.
        Player player = new Player(new ScriptedDie(10, 2, 10, 10, goldRoll));

        Console.SetIn(new StringReader("human\nTestHero\nfighter\nroll\nroll\nroll\nroll\n2\n"));
        Console.SetOut(new StringWriter());

        player.CreateCharacter(CreateMessages());

        Assert.AreEqual(expectedGold, player.Gold);
    }

    [TestMethod]
    [DataRow("English", "Gold: 70")]
    [DataRow("Spanish", "Oro: 70")]
    [DataRow("French", "Or: 70")]
    [DataRow("Italian", "Oro: 70")]
    public void DisplayStats_ShowsGold(string language, string expectedLine)
    {
        Player player = new Player { Gold = 70 };
        StringWriter output = new StringWriter();
        Console.SetOut(output);

        player.DisplayStats(CreateMessages(language));

        StringAssert.Contains(output.ToString(), expectedLine);
    }

    [TestMethod]
    public void CreateCharacter_RaceDoesNotChangeGold()
    {
        // Halfling: Strength d20 - d4, Agility d20 + d4, Health d20, Gold d20.
        Player player = new Player(new ScriptedDie(10, 3, 10, 2, 10, 7));

        Console.SetIn(new StringReader("halfling\nTestHero\nthief\nroll\nroll\nroll\nroll\n2\n"));
        Console.SetOut(new StringWriter());

        player.CreateCharacter(CreateMessages());

        Assert.AreEqual(70, player.Gold);
    }

    [TestMethod]
    public void CreateCharacter_AsksForGoldAfterHealthPoints_AndRemindsOnOtherInput()
    {
        Player player = new Player(new ScriptedDie(10, 2, 10, 10, 7));
        StringWriter output = new StringWriter();

        Console.SetIn(new StringReader("human\nTestHero\nfighter\nroll\nroll\nroll\ngimme\nroll\n2\n"));
        Console.SetOut(output);

        player.CreateCharacter(CreateMessages());

        string text = output.ToString();
        int healthPrompt = text.IndexOf("Next, we will roll for health points. Type 'roll'");
        int goldPrompt = text.IndexOf("Finally, we will roll for your Gold. Type 'roll'");
        int reminder = text.IndexOf("Please type 'roll' to continue.", goldPrompt);

        Assert.IsTrue(healthPrompt >= 0 && goldPrompt > healthPrompt, "Gold prompt should follow the Health Points prompt.");
        Assert.IsTrue(reminder > goldPrompt, "Other input at the Gold prompt should show the 'type roll' reminder.");
        Assert.AreEqual(70, player.Gold);
    }
}
