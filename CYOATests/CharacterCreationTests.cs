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
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        // Human, so no extra Agility roll: Strength d20 + d4 modifier, Agility d20, Health d20.
        Player player = new Player(new ScriptedDie(10, 2, 10, 10));

        Console.SetIn(new StringReader($"human\nTestHero\n{occupation}\nroll\nroll\nroll\n2\n"));
        Console.SetOut(new StringWriter());

        player.CreateCharacter(messages);

        Assert.IsNotNull(player.Weapon);
        Assert.AreEqual(weaponType, player.Weapon.Type);
        Assert.AreEqual(maxDamage, player.Weapon.MaxDamage);
        Assert.AreEqual(asciiArt, player.Weapon.AsciiArt);
    }
}
