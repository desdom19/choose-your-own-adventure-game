using System.IO;

// When console input runs out, every prompt should stop the game instead of re-asking forever.
[TestClass]
[DoNotParallelize]
public class InputEndedTests
{
    private TextReader? _originalIn;
    private TextWriter? _originalOut;

    [TestInitialize]
    public void Setup()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
        Console.SetOut(new StringWriter());
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
    [DataRow("")]                // at language selection
    [DataRow("1\n")]             // at the main menu
    [DataRow("1\n3\n")]          // after an invalid main menu choice
    public void StartGame_Stops_WhenInputEndsBeforeCharacterCreation(string input)
    {
        Console.SetIn(new StringReader(input));

        Assert.ThrowsException<EndOfStreamException>(() => new Game().StartGame());
    }

    [TestMethod]
    public void StartGame_Stops_WhenInputEndsAtThePathPrompt()
    {
        // English, new game, Normal, full character creation, then input ends at the crossroads.
        Console.SetIn(new StringReader("1\n1\n2\nhuman\nTestHero\nfighter\nroll\nroll\nroll\nroll\n2\n"));

        Assert.ThrowsException<EndOfStreamException>(() => new Game().StartGame());
    }

    [TestMethod]
    public void StartCombat_Stops_WhenInputEndsAtTheAttackPrompt()
    {
        // Both sides miss the first round (roll 20 against Strength 1), then the attack prompt is shown.
        Player player = new Player(new ScriptedDie(20))
        {
            Strength = 1,
            HealthPoints = 10,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
        Dragon dragon = new Dragon(new ScriptedDie(1, 1, 20, 20)); // Strength 1, Agility 1, Health 20, attack 20

        Messages messages = new Messages();
        messages.ReadDictionary();

        Console.SetIn(new StringReader(""));

        Assert.ThrowsException<EndOfStreamException>(() => new Combat(player, dragon, messages).StartCombat());
    }
}
