using System.IO;

// Runs a short Final Fight through FinalFight.Run with scripted answers and rolls, and
// returns everything it printed. The rolls, in fight order, all come from the one Die given
// to Run. Test classes that use it touch the console, so they must not run in parallel.
public static class ScriptedFinalFight
{
    public static FinalFightOutcome Play(string input, Player player, Monster monster, Messages messages, int[] rolls, out string output)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;

        try
        {
            Console.SetIn(new StringReader(input));
            using StringWriter writer = new StringWriter();
            Console.SetOut(writer);

            FinalFightOutcome outcome = FinalFight.Run(player, monster, messages, new ScriptedDie(rolls));

            output = writer.ToString().Replace("\r\n", "\n");
            return outcome;
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    public static Messages GetMessages(string language = "English")
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // Joins lines the way the captured output does.
    public static string Lines(params string[] lines) => string.Join("\n", lines);
}
