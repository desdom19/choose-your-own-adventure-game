public static class ConsoleInput
{
    // Console.ReadLine returns null once input has ended (for example, the console closed).
    // Every prompt in the game re-asks on bad input, so without this check they would loop
    // forever. Program catches the exception and exits.
    public static string ReadLine()
    {
        string? line = Console.ReadLine();

        if (line == null)
            throw new EndOfStreamException("Input ended before the game finished.");

        return line.Trim();
    }
}
