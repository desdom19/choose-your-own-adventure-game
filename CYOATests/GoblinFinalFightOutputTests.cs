using System.IO;
using System.Text.RegularExpressions;

// The text a Goblin Final Fight prints: whole fights with scripted rolls and console
// input, the Goblin's endings in every language, and no Dragon text or raw keys.
[TestClass]
[DoNotParallelize]
public class GoblinFinalFightOutputTests
{
    private static readonly string[] Languages = { "English", "Spanish", "French", "Italian" };

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

    private static Messages GetMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // A Fighter carrying both Merchant items: the Magical Weapon and Armor.
    private static Player CreatePlayer(Die die, int healthPoints)
    {
        OccupationEquipment equipment = OccupationEquipment.For("Fighter")!;

        return new Player(die)
        {
            Name = "TestHero",
            Race = "Elf",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = healthPoints,
            Weapon = equipment.CreateMagicalWeapon(),
            Armor = equipment.CreateArmor()
        };
    }

    private static string Capture(string input, Action action)
    {
        Console.SetIn(new StringReader(input));
        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);
        action();
        return writer.ToString().Replace("\r\n", "\n");
    }

    private static void AssertSameText(string expected, string actual)
    {
        Assert.AreEqual(expected.Replace("\r\n", "\n").TrimEnd(), actual.TrimEnd());
    }

    // Runs the whole Final Fight, from the path narrative to the ending.
    private static string PlayFinalFight(string input, Player player, Goblin goblin, Messages messages, out FinalFightOutcome outcome)
    {
        FinalFightOutcome result = default;
        string output = Capture(input, () => result = FinalFight.Run(player, goblin, messages, new ScriptedDie()));
        outcome = result;
        return output;
    }

    // The fight from the stats before the first Round on: the encounter before it is covered
    // by the Dragon's tests.
    private static void AssertSameTextFromTheStats(string expected, string actual)
    {
        expected = expected.Replace("\r\n", "\n").TrimEnd();
        int start = actual.IndexOf(expected.Split('\n')[0]);

        Assert.AreNotEqual(-1, start, $"The stats before the first Round were not shown:\n{actual}");
        Assert.AreEqual(expected, actual[start..].TrimEnd());
    }

    // Goblin: Strength 10, Agility 5 + 5, Health 6. Round 1: taunt 3, attack hits (1),
    // player fails to dodge (20), damage 7. Player: round 1 hits (1), Goblin dodges (5);
    // round 2 hits (1), no dodge (20), damage 6.
    private static string PlayWonFight(string language, out FinalFightOutcome outcome)
    {
        Goblin goblin = new Goblin(new ScriptedDie(10, 5, 6, 3, 1, 20, 7));
        Player player = CreatePlayer(new ScriptedDie(1, 5, 1, 20, 6), 20);
        return PlayFinalFight("a\na\n", player, goblin, GetMessages(language), out outcome);
    }

    // The player misses (20); the Goblin hits (1), the dodge fails (20) and it deals 8.
    private static string PlayLostFight(string language, out FinalFightOutcome outcome)
    {
        Goblin goblin = new Goblin(new ScriptedDie(10, 5, 6, 1, 20, 8));
        Player player = CreatePlayer(new ScriptedDie(20), 3);
        return PlayFinalFight("a\n", player, goblin, GetMessages(language), out outcome);
    }

    [TestMethod]
    public void WonFight_PrintsExactText()
    {
        string output = PlayWonFight("English", out FinalFightOutcome outcome);

        Assert.AreEqual(FinalFightOutcome.Won, outcome);
        AssertSameTextFromTheStats("""
            ===== Combat Stats =====
            --- Player ---
            Name: TestHero
            Strength: 15
            Agility: 12
            Health Points: 20
            Protection: 3
            Weapon: flaming sword
            Weapon's Maximum Damage Points: 16

            --- Goblin ---
            Name: Gritknuckle
            Strength: 10
            Agility: 10
            Health Points: 6
            Weapon: rusty cleaver
            Weapon's Maximum Damage Points: 9
            As a Fighter, you withdraw your flaming sword and attack the goblin.
            -)=====>~*~*
            You hit Gritknuckle.
            Gritknuckle successfully defends your attack.
            Over here, Elf! No, over here! Hee hee!
            Gritknuckle attacks TestHero with rusty cleaver!
            |=[####]
            Gritknuckle hits TestHero.
            Gritknuckle deals 7 damage.
            Armor (enchanted plate armor): 3 damage absorbed, 4 damage taken.
            TestHero's health points are now 16.
            Enter 'a' to attack or 'r' to retreat.
            As a Fighter, you withdraw your flaming sword and attack the goblin.
            -)=====>~*~*
            You hit Gritknuckle.
            You deal 6 damage.
            Gritknuckle's health points are now 0.
            With a last squeal of 'Eeek!', you have defeated the goblin.
            With the goblin beaten, the west road is safe again. You return home a hero. The End.
            """, output);
    }

    [TestMethod]
    public void LostFight_PrintsExactText()
    {
        string output = PlayLostFight("English", out FinalFightOutcome outcome);

        Assert.AreEqual(FinalFightOutcome.Lost, outcome);
        AssertSameTextFromTheStats("""
            ===== Combat Stats =====
            --- Player ---
            Name: TestHero
            Strength: 15
            Agility: 12
            Health Points: 3
            Protection: 3
            Weapon: flaming sword
            Weapon's Maximum Damage Points: 16

            --- Goblin ---
            Name: Gritknuckle
            Strength: 10
            Agility: 10
            Health Points: 6
            Weapon: rusty cleaver
            Weapon's Maximum Damage Points: 9
            As a Fighter, you withdraw your flaming sword and attack the goblin.
            -)=====>~*~*
            You missed Gritknuckle.
            Gritknuckle attacks TestHero with rusty cleaver!
            |=[####]
            Gritknuckle hits TestHero.
            Gritknuckle deals 8 damage.
            Armor (enchanted plate armor): 3 damage absorbed, 5 damage taken.
            TestHero's health points are now 0.
            TestHero has been defeated.
            The goblin cackles and rummages through your pockets as your eyesight fades. You are defeated. Your family mourns their loss. The End.
            """, output);
    }

    [TestMethod]
    public void RetreatMidFight_EndsWithTheSharedRetreatText()
    {
        // The player hits for 2 (Goblin to 4 Health Points), the Goblin replies (2) and
        // misses (15), then the player retreats.
        Goblin goblin = new Goblin(new ScriptedDie(10, 5, 6, 2, 15));
        Player player = CreatePlayer(new ScriptedDie(1, 20, 2), 20);
        Messages messages = GetMessages("English");

        string output = PlayFinalFight("a\nr\n", player, goblin, messages, out FinalFightOutcome outcome);

        Assert.AreEqual(FinalFightOutcome.Retreated, outcome);
        Assert.AreEqual(4, goblin.HealthPoints);
        StringAssert.EndsWith(output.TrimEnd(), string.Join("\n",
            "You hit Gritknuckle.",
            "You deal 2 damage.",
            "Gritknuckle's health points are now 4.",
            "You'll pay for that, you big lump!",
            "Gritknuckle attacks TestHero with rusty cleaver!",
            "|=[####]",
            "Gritknuckle missed TestHero.",
            "Enter 'a' to attack or 'r' to retreat.",
            messages.GetMessage("retreat_combat")));
    }

    [TestMethod]
    [DataRow("English", "With the goblin beaten, the west road is safe again. You return home a hero. The End.")]
    [DataRow("Spanish", "Con el goblin vencido, el camino del oeste vuelve a ser seguro. Regresas a casa como un heroe. Fin.")]
    [DataRow("French", "Le gobelin vaincu, le chemin de l'ouest est de nouveau sans danger. Vous rentrez chez vous en heros. Fin.")]
    [DataRow("Italian", "Con il goblin sconfitto, il sentiero a ovest torna sicuro. Torni a casa da eroe. Fine.")]
    public void VictoryEnding_PrintsExactText(string language, string expected)
    {
        string output = PlayWonFight(language, out FinalFightOutcome outcome);

        Assert.AreEqual(FinalFightOutcome.Won, outcome);
        StringAssert.EndsWith(output.TrimEnd(), "\n" + expected);
    }

    [TestMethod]
    [DataRow("English", "The goblin cackles and rummages through your pockets as your eyesight fades. You are defeated. Your family mourns their loss. The End.")]
    [DataRow("Spanish", "El goblin se carcajea y rebusca en tus bolsillos mientras tu vista se desvanece. Has sido derrotado. Tu familia llora tu perdida. Fin.")]
    [DataRow("French", "Le gobelin ricane et fouille vos poches tandis que votre vue s'eteint. Vous etes vaincu. Votre famille pleure votre perte. Fin.")]
    [DataRow("Italian", "Il goblin sghignazza e fruga nelle tue tasche mentre la tua vista si spegne. Sei sconfitto. La tua famiglia piange la tua perdita. Fine.")]
    public void DefeatEnding_PrintsExactText(string language, string expected)
    {
        string output = PlayLostFight(language, out FinalFightOutcome outcome);

        Assert.AreEqual(FinalFightOutcome.Lost, outcome);
        StringAssert.EndsWith(output.TrimEnd(), "\n" + expected);
    }

    // A retreat at the encounter prompt ends with the retreat text every Monster shares.
    [TestMethod]
    [DataRow("English", "Enter choice: You quickly retreat. You come home poor but alive to adventure another day. The End.")]
    [DataRow("Spanish", "Escribe tu opcion: Te has retirado de la batalla.")]
    [DataRow("French", "Entrez votre choix : Vous vous etes retire du combat.")]
    [DataRow("Italian", "Inserisci la tua scelta: Ti ritiri in fretta. Torni a casa povero ma vivo, pronto per una nuova avventura. Fine.")]
    public void RetreatEnding_IsTheSharedRetreatText(string language, string expected)
    {
        Goblin goblin = new Goblin(new ScriptedDie(10, 5, 6));
        Player player = CreatePlayer(new ScriptedDie(), 20);

        string output = PlayFinalFight("r\n", player, goblin, GetMessages(language), out FinalFightOutcome outcome);

        Assert.AreEqual(FinalFightOutcome.Retreated, outcome);
        StringAssert.EndsWith(output.TrimEnd(), "\n" + expected);
    }

    // The whole encounter, from the path narrative to the ending, so every kind of Goblin
    // text is shown at least once.
    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void GoblinEncounter_ShowsNoRawKeysAndNoDragonText(string language)
    {
        Goblin goblin = new Goblin(new ScriptedDie(10, 5, 6, 3, 1, 20, 7));
        Player player = CreatePlayer(new ScriptedDie(1, 5, 1, 20, 6), 20);
        Messages messages = GetMessages(language);

        string output = PlayFinalFight("a\na\n", player, goblin, messages, out FinalFightOutcome outcome);

        Assert.AreEqual(FinalFightOutcome.Won, outcome);
        StringAssert.Contains(output, messages.GetMessage("goblin_victory"));
        Assert.IsFalse(Regex.IsMatch(output, @"\[[a-z0-9_]+\]"), $"Raw message key shown:\n{output}");
        AssertNoDragon(output, language);
    }

    [TestMethod]
    public void EveryGoblinMessage_NeverMentionsTheDragon()
    {
        string jsonPath = Path.Combine(AppContext.BaseDirectory, "language_data.json");
        using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(jsonPath));

        foreach (string language in Languages)
        {
            var goblinMessages = doc.RootElement.GetProperty(language).GetProperty("dictionary")
                .EnumerateObject()
                .Where(entry => entry.Name.StartsWith("goblin_"))
                .ToList();

            Assert.AreNotEqual(0, goblinMessages.Count, $"{language} has no Goblin messages.");

            foreach (var entry in goblinMessages)
                AssertNoDragon(entry.Value.GetString() ?? "", $"{language} {entry.Name}");
        }
    }

    private static void AssertNoDragon(string text, string context)
    {
        // "drag" matches both "dragon" (English, Spanish, French) and "drago" (Italian).
        Assert.IsFalse(text.Contains("drag", StringComparison.OrdinalIgnoreCase), $"{context} mentions the Dragon:\n{text}");
        Assert.IsFalse(text.Contains("Smolderfang"), $"{context} mentions Smolderfang:\n{text}");
    }
}
