// CYOATests/MessagesTests.cs
[TestClass]
public class MessagesTests
{
    [TestMethod]
    public void Constructor_DefaultLanguageIsEnglish()
    {
        Messages messages = new Messages();
        Assert.AreEqual("English", messages.CurrentLanguage);
    }

    [TestMethod]
    public void SetCurrentLanguage_SetsLanguageToSpanish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        Assert.AreEqual("Spanish", messages.CurrentLanguage);
    }

    [TestMethod]
    public void SetCurrentLanguage_SetsLanguageToFrench()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("French");
        Assert.AreEqual("French", messages.CurrentLanguage);
    }

    [TestMethod]
    public void SetCurrentLanguage_SetsLanguageToEnglish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        messages.SetCurrentLanguage("English");
        Assert.AreEqual("English", messages.CurrentLanguage);
    }

    [TestMethod]
    public void ReadDictionary_LoadsEnglishWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        string result = messages.GetMessage("welcome");
        Assert.AreEqual("Welcome to Choose Your Adventure Game", result);
    }

    [TestMethod]
    public void ReadDictionary_LoadsSpanishWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();
        string result = messages.GetMessage("welcome");
        Assert.AreEqual("Bienvenido al Juego de Elige Tu Aventura", result);
    }

    [TestMethod]
    public void ReadDictionary_LoadsFrenchWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("French");
        messages.ReadDictionary();
        string result = messages.GetMessage("welcome");
        Assert.AreEqual("Bienvenue dans Choisissez Votre Aventure", result);
    }

    [TestMethod]
    public void GetMessage_ReturnsFallbackForUnknownKey()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        string result = messages.GetMessage("unknown_key");
        Assert.AreEqual("[unknown_key]", result);
    }

    [TestMethod]
    public void GetMessage_ReturnsDifferentMessageAfterLanguageSwitch()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        string english = messages.GetMessage("welcome");

        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();
        string spanish = messages.GetMessage("welcome");

        Assert.AreNotEqual(english, spanish);
    }

    [TestMethod]
    public void ReadDictionary_LoadsGoodbyeMessageInEachLanguage()
    {
        Messages messages = new Messages();

        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Adventure ends. Bye.", messages.GetMessage("goodbye"));

        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();
        Assert.AreEqual("Adios!", messages.GetMessage("goodbye"));

        messages.SetCurrentLanguage("French");
        messages.ReadDictionary();
        Assert.AreEqual("Au revoir!", messages.GetMessage("goodbye"));
    }

    [TestMethod]
    public void ReadDictionary_AllRequiredKeysExistInEnglish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        string[] requiredKeys = { "welcome", "menu",
                                   "choose_path", "path_north", "path_south", "attack_prompt",
                                   "victory", "defeat", "retreat", "invalid", "goodbye" };

        foreach (string key in requiredKeys)
        {
            string result = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", result, $"Key '{key}' is missing from English dictionary.");
        }
    }

    [TestMethod]
    public void ReadDictionary_AllRequiredKeysExistInSpanish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();

        string[] requiredKeys = { "welcome", "menu",
                                   "choose_path", "path_north", "path_south", "attack_prompt",
                                   "victory", "defeat", "retreat", "invalid", "goodbye" };

        foreach (string key in requiredKeys)
        {
            string result = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", result, $"Key '{key}' is missing from Spanish dictionary.");
        }
    }

    [TestMethod]
    public void ReadDictionary_AllRequiredKeysExistInFrench()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("French");
        messages.ReadDictionary();

        string[] requiredKeys = { "welcome", "menu",
                                   "choose_path", "path_north", "path_south", "attack_prompt",
                                   "victory", "defeat", "retreat", "invalid", "goodbye" };

        foreach (string key in requiredKeys)
        {
            string result = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", result, $"Key '{key}' is missing from French dictionary.");
        }
    }

    // --- Italian ---

    private static Messages CreateItalianMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Italian");
        messages.ReadDictionary();
        return messages;
    }

    [TestMethod]
    public void ReadDictionary_LoadsItalianWelcomeMessage()
    {
        Assert.AreEqual("Benvenuto in Scegli la Tua Avventura", CreateItalianMessages().GetMessage("welcome"));
    }

    [TestMethod]
    [DataRow("umano", "Human", "Umano")]
    [DataRow("ELFO", "Elf", "Elfo")]
    [DataRow("nano", "Dwarf", "Nano")]
    [DataRow("mezzuomo", "Halfling", "Mezzuomo")]
    public void Italian_RaceInput_NormalizesAndDisplays(string input, string canonical, string display)
    {
        Messages messages = CreateItalianMessages();
        Assert.IsTrue(messages.IsValidRace(input));
        Assert.AreEqual(canonical, messages.NormalizeRace(input));
        Assert.AreEqual(display, messages.TranslateRaceForDisplay(canonical));
    }

    [TestMethod]
    [DataRow("guerriero", "Fighter", "Guerriero")]
    [DataRow("MAGO", "Magician", "Mago")]
    [DataRow("ladro", "Thief", "Ladro")]
    [DataRow("arciere", "Archer", "Arciere")]
    public void Italian_OccupationInput_NormalizesAndDisplays(string input, string canonical, string display)
    {
        Messages messages = CreateItalianMessages();
        Assert.IsTrue(messages.IsValidOccupation(input));
        Assert.AreEqual(canonical, messages.NormalizeOccupation(input));
        Assert.AreEqual(display, messages.TranslateOccupationForDisplay(canonical));
    }

    [TestMethod]
    [DataRow("long sword", "spada lunga")]
    [DataRow("lightning bolt spell", "incantesimo del fulmine")]
    [DataRow("dagger", "pugnale")]
    [DataRow("long bow", "arco lungo")]
    [DataRow("claws", "artigli")]
    public void Italian_Weapons_AreTranslated(string weapon, string display)
    {
        Assert.AreEqual(display, CreateItalianMessages().TranslateWeaponForDisplay(weapon));
    }

    // Italian text must have the same keys as English and the same {0}-style
    // placeholders, because the game passes each message to string.Format.
    [TestMethod]
    public void Italian_Dictionary_MatchesEnglishKeysAndPlaceholders()
    {
        string jsonPath = Path.Combine(AppContext.BaseDirectory, "language_data.json");
        using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(jsonPath));

        Dictionary<string, string> english = ReadSection(doc, "English");
        Dictionary<string, string> italian = ReadSection(doc, "Italian");

        CollectionAssert.AreEquivalent(english.Keys.ToList(), italian.Keys.ToList());

        foreach (KeyValuePair<string, string> entry in english)
        {
            CollectionAssert.AreEquivalent(
                Placeholders(entry.Value),
                Placeholders(italian[entry.Key]),
                $"Placeholders differ for key '{entry.Key}'.");
        }
    }

    private static Dictionary<string, string> ReadSection(System.Text.Json.JsonDocument doc, string language)
    {
        return doc.RootElement.GetProperty(language).GetProperty("dictionary").EnumerateObject()
            .ToDictionary(entry => entry.Name, entry => entry.Value.GetString() ?? string.Empty);
    }

    private static List<string> Placeholders(string text)
    {
        return System.Text.RegularExpressions.Regex.Matches(text, @"\{\d+\}")
            .Select(match => match.Value)
            .ToList();
    }

    // --- IsValidRace ---

    [TestMethod]
    public void IsValidRace_ReturnsTrue_ForHuman()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidRace("human"));
    }

    [TestMethod]
    public void IsValidRace_ReturnsTrue_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidRace("HUMAN"));
    }

    [TestMethod]
    public void IsValidRace_ReturnsFalse_ForInvalidRace()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidRace("orc"));
    }

    [TestMethod]
    public void IsValidRace_ReturnsFalse_ForNull()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidRace(null));
    }

    [TestMethod]
    public void NormalizeRace_ReturnsCanonical_ForHuman()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Human", messages.NormalizeRace("human"));
    }

    [TestMethod]
    public void NormalizeRace_ReturnsCanonical_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Human", messages.NormalizeRace("HUMAN"));
    }

    // --- IsValidOccupation ---

    [TestMethod]
    public void IsValidOccupation_ReturnsTrue_ForFighter()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidOccupation("fighter"));
    }

    [TestMethod]
    public void IsValidOccupation_ReturnsTrue_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidOccupation("FIGHTER"));
    }

    [TestMethod]
    public void IsValidOccupation_ReturnsFalse_ForInvalidOccupation()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidOccupation("wizard"));
    }

    [TestMethod]
    public void IsValidOccupation_ReturnsFalse_ForNull()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidOccupation(null));
    }

    [TestMethod]
    public void NormalizeOccupation_ReturnsCanonical_ForFighter()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Fighter", messages.NormalizeOccupation("fighter"));
    }

    [TestMethod]
    public void NormalizeOccupation_ReturnsCanonical_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Fighter", messages.NormalizeOccupation("FIGHTER"));
    }
}
