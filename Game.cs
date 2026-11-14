public class Game
{
    public void StartGame()
    {
        // Title screen
        Console.WriteLine("=====================================");
        Console.WriteLine("Welcome to Choose Your Adventure Game");
        Console.WriteLine("=====================================\n");

        // Instantiate Die first (per sequence diagram)
        Die die = new Die();

        // Language selection shown before Messages dictionary is loaded
        Console.WriteLine("Select language / Selecciona idioma / Choisissez la langue / Scegli la lingua:");
        Console.WriteLine("1. English  2. Espanol  3. Francais  4. Italiano");
        Console.Write("Enter choice: ");
        string langInput = ConsoleInput.ReadLine();
        string selectedLanguage = langInput switch { "2" => "Spanish", "3" => "French", "4" => "Italian", _ => "English" };

        // Initialize Messages with selected language (per sequence diagram)

        Messages messages = new Messages();
        messages.SetCurrentLanguage(selectedLanguage);
        messages.ReadDictionary();
        // Display welcome message
        Console.WriteLine();
        Console.WriteLine(messages.GetMessage("welcome"));
        Console.WriteLine();

        // Main menu loop
        bool running = true;
        while (running)
        {
            Console.WriteLine(messages.GetMessage("menu"));
            Console.Write(messages.GetMessage("enter_choice"));
            string choice = ConsoleInput.ReadLine();

            if (choice == "1")
            {
                Difficulty difficulty = PromptForDifficulty(messages);
                Dragon dragon = new Dragon(die, difficulty);
                Goblin goblin = new Goblin(die, difficulty);

                Player player = new Player();
                player.CreateCharacter(messages);

                bool inAdventureMenu = true;
                while (inAdventureMenu)
                {
                    string selectedPath = PromptForPath(messages);

                    if (selectedPath == "exit")
                    {
                        Console.WriteLine(messages.GetMessage("adventure_ends"));
                        running = false;
                        inAdventureMenu = false;
                    }
                    else if (selectedPath == "south")
                    {
                        Console.WriteLine(messages.GetMessage("south_path_merchant_narrative"));
                        Merchant.Visit(player, messages);
                        Console.WriteLine(messages.GetMessage("merchant_back_at_crossroads"));
                    }
                    else if (selectedPath == "north" || selectedPath == "west")
                    {
                        // Every outcome ends the adventure.
                        FinalFight.Run(player, selectedPath == "north" ? dragon : goblin, messages, die);

                        running = false;
                        inAdventureMenu = false;
                    }
                }
            }
            else if (choice == "2")
            {
                Console.WriteLine(messages.GetMessage("goodbye"));
                running = false;
            }
            else
            {
                Console.WriteLine(messages.GetMessage("invalid"));
            }
        }
    }

    public bool IsValidMenuChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim();
        return trimmed == "1" || trimmed == "2";
    }

    public bool IsValidPathChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        return trimmed == "n" || trimmed == "north" ||
               trimmed == "s" || trimmed == "south" ||
               trimmed == "w" || trimmed == "west" ||
               trimmed == "e" || trimmed == "exit";
    }

    public bool IsValidCombatChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        return trimmed == "a" || trimmed == "attack" ||
               trimmed == "r" || trimmed == "retreat";
    }

    private Difficulty PromptForDifficulty(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("difficulty_prompt"));
            Console.Write(messages.GetMessage("enter_choice"));
            string input = ConsoleInput.ReadLine();

            if (input == "1")
                return ConfirmDifficulty(Difficulty.Easy, "difficulty_easy", messages);

            if (input == "2")
                return ConfirmDifficulty(Difficulty.Normal, "difficulty_normal", messages);

            if (input == "3")
                return ConfirmDifficulty(Difficulty.Hard, "difficulty_hard", messages);

            Console.WriteLine(messages.GetMessage("invalid"));
        }
    }

    private static Difficulty ConfirmDifficulty(Difficulty difficulty, string nameKey, Messages messages)
    {
        Console.WriteLine(string.Format(messages.GetMessage("difficulty_chosen"), messages.GetMessage(nameKey)));
        return difficulty;
    }

    private string PromptForPath(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("path_prompt_full"));
            Console.Write(messages.GetMessage("enter_choice"));
            string input = ConsoleInput.ReadLine().ToLower();

            if (input == "north" || input == "n")
                return "north";

            if (input == "south" || input == "s")
                return "south";

            if (input == "west" || input == "w")
                return "west";

            if (input == "exit" || input == "e")
                return "exit";

            Console.WriteLine(messages.GetMessage("path_invalid"));
        }
    }
}
