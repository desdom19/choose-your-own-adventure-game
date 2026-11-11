Game game = new Game();

try
{
    game.StartGame();
}
catch (EndOfStreamException)
{
    // Input ended (for example, the console closed) before the game could finish: just exit.
}
