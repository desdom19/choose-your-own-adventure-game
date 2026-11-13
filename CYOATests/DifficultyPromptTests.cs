using System.IO;

// The Difficulty prompt at the start of a new adventure, driven through the console at Game.StartGame.
[TestClass]
[DoNotParallelize]
public class DifficultyPromptTests
{
    private const string Prompt = "Choose a Difficulty; 1. Easy, 2. Normal or 3. Hard. Type '1', '2' or '3'";
    private const string RacePrompt = "What race of character do you want to be?";
    private const string InvalidChoice = "Invalid choice. Please try again.";
    private const string AnyConfirmation = "You chose the";
    private const string Goodbye = "Adventure ends. Bye.";

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

    // Runs the game until input runs out and returns everything it printed.
    private static string RunUntilInputEnds(string input)
    {
        Console.SetIn(new StringReader(input));
        StringWriter output = new StringWriter();
        Console.SetOut(output);

        Assert.ThrowsException<EndOfStreamException>(() => new Game().StartGame());

        return output.ToString();
    }

    [TestMethod]
    public void StartGame_AsksForDifficulty_AfterTheMainMenuAndBeforeTheRacePrompt()
    {
        // English, new game, Normal, then input ends at the race prompt.
        string printed = RunUntilInputEnds("1\n1\n2\n");

        int menu = printed.IndexOf("Choose; 1. Create Character or 2. Exit.");
        int prompt = printed.IndexOf(Prompt);
        int race = printed.IndexOf(RacePrompt);

        Assert.IsTrue(menu >= 0, "main menu not shown");
        Assert.IsTrue(prompt > menu, "Difficulty prompt not shown after the main menu");
        Assert.IsTrue(race > prompt, "race prompt not shown after the Difficulty prompt");
    }

    [TestMethod]
    [DataRow("1", "You chose the Easy Difficulty.")]
    [DataRow("2", "You chose the Normal Difficulty.")]
    [DataRow("3", "You chose the Hard Difficulty.")]
    [DataRow(" 3 ", "You chose the Hard Difficulty.")]
    public void StartGame_AcceptsEachDifficulty_AndConfirmsIt(string answer, string confirmation)
    {
        string printed = RunUntilInputEnds($"1\n1\n{answer}\n");

        int prompt = printed.IndexOf(Prompt);
        int confirmed = printed.IndexOf(confirmation);
        int race = printed.IndexOf(RacePrompt);

        Assert.IsTrue(confirmed > prompt && prompt >= 0, "confirmation not shown after the Difficulty prompt");
        Assert.IsTrue(race > confirmed, "race prompt not shown after the confirmation");
        Assert.AreEqual(1, CountOf(printed, Prompt), "Difficulty prompt asked more than once");
        Assert.IsFalse(printed.Contains(InvalidChoice), "valid answer treated as invalid");
    }

    [TestMethod]
    [DataRow("4")]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("easy")]
    [DataRow("e")]
    public void StartGame_AsksForDifficultyAgain_AfterAnInvalidOrEmptyAnswer(string answer)
    {
        string printed = RunUntilInputEnds($"1\n1\n{answer}\n2\n");

        int firstPrompt = printed.IndexOf(Prompt);
        int invalid = printed.IndexOf(InvalidChoice, firstPrompt);
        int secondPrompt = printed.IndexOf(Prompt, firstPrompt + Prompt.Length);
        int confirmed = printed.IndexOf("You chose the Normal Difficulty.");

        Assert.IsTrue(invalid > firstPrompt, "invalid answer not reported");
        Assert.IsTrue(secondPrompt > invalid, "Difficulty prompt not asked again");
        Assert.IsTrue(confirmed > secondPrompt, "second answer not accepted");
        Assert.AreEqual(1, CountOf(printed, AnyConfirmation), "invalid answer picked a Difficulty");
    }

    [TestMethod]
    public void StartGame_Stops_WhenInputEndsAtTheDifficultyPrompt()
    {
        string printed = RunUntilInputEnds("1\n1\n");

        StringAssert.Contains(printed, Prompt);
        Assert.IsFalse(printed.Contains(RacePrompt), "game went on past the Difficulty prompt");
    }

    [TestMethod]
    public void StartGame_NeverAsksForDifficulty_WhenThePlayerExits()
    {
        Console.SetIn(new StringReader("1\n2\n"));
        StringWriter output = new StringWriter();
        Console.SetOut(output);

        new Game().StartGame();

        string printed = output.ToString();
        StringAssert.Contains(printed, Goodbye);
        Assert.IsFalse(printed.Contains("Difficulty"), "Difficulty mentioned when exiting");
    }

    [TestMethod]
    [DataRow("2", "Elige una Dificultad; 1. Facil, 2. Normal o 3. Dificil. Escribe '1', '2' o '3'", "Elegiste la Dificultad Dificil.")]
    [DataRow("3", "Choisissez une Difficulte; 1. Facile, 2. Normale ou 3. Difficile. Tapez '1', '2' ou '3'", "Vous avez choisi la Difficulte Difficile.")]
    [DataRow("4", "Scegli una Difficolta; 1. Facile, 2. Normale o 3. Difficile. Scrivi '1', '2' o '3'", "Hai scelto la Difficolta Difficile.")]
    public void StartGame_AsksForDifficulty_InThePlayersLanguage(string language, string localizedPrompt, string localizedConfirmation)
    {
        string printed = RunUntilInputEnds($"{language}\n1\n3\n");

        StringAssert.Contains(printed, localizedPrompt);
        StringAssert.Contains(printed, localizedConfirmation);
        Assert.IsFalse(printed.Contains("[difficulty"), "raw message key shown");
    }

    private static int CountOf(string text, string part)
    {
        int count = 0;
        for (int i = text.IndexOf(part); i >= 0; i = text.IndexOf(part, i + part.Length))
            count++;
        return count;
    }
}
