using System.IO;

[TestClass]
[DoNotParallelize]
public class MerchantTests
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

    private static Messages CreateMessages(string language = "English")
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // Runs one Merchant visit with the given lines of input and returns everything printed.
    private static string Visit(Player player, string input, string language = "English")
    {
        StringWriter output = new StringWriter();
        Console.SetIn(new StringReader(input));
        Console.SetOut(output);

        Merchant.Visit(player, CreateMessages(language));

        return output.ToString();
    }

    [TestMethod]
    [DataRow("Fighter", "enchanted plate armor")]
    [DataRow("Magician", "warded robe")]
    [DataRow("Thief", "shadow leathers")]
    [DataRow("Archer", "elven cloak")]
    public void BuyingArmor_Takes70Gold_AndGivesTheOccupationsArmor(string occupation, string armorName)
    {
        Player player = new Player { Occupation = occupation, Gold = 100 };

        Visit(player, "1\n3\n");

        Assert.AreEqual(30, player.Gold);
        Assert.IsNotNull(player.Armor);
        Assert.AreEqual(armorName, player.Armor.Name);
        Assert.AreEqual(3, player.Armor.Protection);
    }

    [TestMethod]
    public void BuyingArmorWith69Gold_IsRefused_AndPlayerStaysUntilLeave()
    {
        Player player = new Player { Occupation = "Fighter", Gold = 69 };

        string output = Visit(player, "1\n3\n");

        Assert.AreEqual(69, player.Gold);
        Assert.IsNull(player.Armor);
        int refusal = output.IndexOf("You don't have enough Gold for that.");
        Assert.IsTrue(refusal >= 0, "Expected the 'not enough Gold' message.");
        Assert.IsTrue(output.IndexOf("3. Leave", refusal) > refusal, "The menu should be shown again after a refusal.");
    }

    [TestMethod]
    public void PickingArmorWhenAlreadyOwned_IsRefused_AndGoldDoesNotChange()
    {
        Armor owned = new Armor("enchanted plate armor", 3);
        Player player = new Player { Occupation = "Fighter", Gold = 100, Armor = owned };

        string output = Visit(player, "1\n3\n");

        StringAssert.Contains(output, "You already have that.");
        Assert.AreEqual(100, player.Gold);
        Assert.AreSame(owned, player.Armor);
    }

    [TestMethod]
    [DataRow("2")]
    [DataRow("armor")]
    [DataRow("")]
    public void InvalidInput_ShowsInvalidChoice_AndTheMenuAgain(string invalidInput)
    {
        Player player = new Player { Occupation = "Thief", Gold = 100 };

        string output = Visit(player, $"{invalidInput}\n3\n");

        int invalid = output.IndexOf("Invalid choice. Please try again.");
        Assert.IsTrue(invalid >= 0, "Expected the 'Invalid choice' message.");
        Assert.IsTrue(output.IndexOf("1. Armor: shadow leathers (70 Gold)", invalid) > invalid,
            "The menu should be shown again after invalid input.");
        Assert.AreEqual(100, player.Gold);
        Assert.IsNull(player.Armor);
    }

    [TestMethod]
    public void LeavingWithoutBuying_ChangesNothing()
    {
        Player player = new Player { Occupation = "Archer", Gold = 100 };

        Visit(player, "3\n");

        Assert.AreEqual(100, player.Gold);
        Assert.IsNull(player.Armor);
    }

    [TestMethod]
    public void OnALaterVisit_ArmorIsNotOfferedOnceOwned()
    {
        Player player = new Player { Occupation = "Magician", Gold = 100 };
        string firstVisit = Visit(player, "1\n3\n");

        string secondVisit = Visit(player, "3\n");

        StringAssert.Contains(firstVisit, "1. Armor: warded robe (70 Gold)");
        Assert.IsFalse(secondVisit.Contains("1. Armor:"), "Owned Armor should not be offered again.");
        StringAssert.Contains(secondVisit, "3. Leave");
    }

    [TestMethod]
    public void PlayerWith10Gold_StillSeesTheFullScene_AndCanLeave()
    {
        Player player = new Player { Occupation = "Fighter", Gold = 10 };

        string output = Visit(player, "3\n");

        StringAssert.Contains(output, "Welcome, traveler!");
        StringAssert.Contains(output, "You have 10 Gold.");
        StringAssert.Contains(output, "1. Armor: enchanted plate armor (70 Gold)");
        StringAssert.Contains(output, "3. Leave");
        Assert.AreEqual(10, player.Gold);
    }

    [TestMethod]
    public void BuyingArmor_ConfirmsThePurchase_InThePlayersLanguage()
    {
        Player player = new Player { Occupation = "Archer", Gold = 100 };

        string output = Visit(player, "1\n3\n", "Spanish");

        StringAssert.Contains(output, "1. Armadura: capa elfica (70 de Oro)");
        StringAssert.Contains(output, "Compras Armadura (capa elfica) por 70 de Oro.");
    }
}
