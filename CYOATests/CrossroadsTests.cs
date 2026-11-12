using System.IO;

// Routing at the Crossroads, driven through the console at Game.StartGame.
[TestClass]
[DoNotParallelize]
public class CrossroadsTests
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
    public void StartGame_WestPath_LeadsToTheGoblin_AndRetreatingEndsTheGame()
    {
        // English, new game, full character creation, west, then retreat at the encounter prompt.
        // Retreating before combat never rolls a die, so the real Die inside Game doesn't matter.
        Console.SetIn(new StringReader("1\n1\nhuman\nTestHero\nfighter\nroll\nroll\nroll\nroll\n2\nw\nr\n"));
        StringWriter output = new StringWriter();
        Console.SetOut(output);

        // Returning, rather than running out of input, shows the game ended after the retreat.
        new Game().StartGame();

        string printed = output.ToString();
        StringAssert.Contains(printed, "You take the West path. The trail drops into a damp ravine");
        StringAssert.Contains(printed, "The goblin cackles 'I am Gritknuckle. Your boots and your purse are mine!'");
        StringAssert.Contains(printed, "You quickly retreat. You come home poor but alive to adventure another day. The End.");
    }
}
