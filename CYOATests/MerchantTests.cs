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

    // A player as character creation leaves them: holding their occupation's starting weapon.
    private static Player CreatePlayer(string occupation, int gold)
    {
        return new Player
        {
            Occupation = occupation,
            Gold = gold,
            Weapon = OccupationEquipment.For(occupation)!.CreateStartingWeapon(),
        };
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
    [DataRow("Fighter", "flaming sword", 16)]
    [DataRow("Magician", "chain lightning spell", 16)]
    [DataRow("Thief", "venom dagger", 10)]
    [DataRow("Archer", "elven bow", 12)]
    public void BuyingTheMagicalWeapon_Takes90Gold_AndGivesTheOccupationsMagicalWeapon(
        string occupation, string weaponName, int maxDamage)
    {
        Player player = CreatePlayer(occupation, 100);
        Weapon startingWeapon = player.Weapon!;

        string output = Visit(player, "2\n3\n");

        Assert.AreEqual(10, player.Gold);
        Assert.IsNotNull(player.Weapon);
        Assert.AreNotSame(startingWeapon, player.Weapon);
        Assert.AreEqual(weaponName, player.Weapon.Type);
        Assert.AreEqual(maxDamage, player.Weapon.MaxDamage);
        StringAssert.Contains(output, $"You buy a Magical Weapon ({weaponName}) for 90 Gold.");

        Weapon nextStartingWeapon = OccupationEquipment.For(occupation)!.CreateStartingWeapon();
        Assert.AreEqual(startingWeapon.Type, nextStartingWeapon.Type, "The starting weapon definition should not change.");
        Assert.AreEqual(startingWeapon.MaxDamage, nextStartingWeapon.MaxDamage);
    }

    [TestMethod]
    public void AfterBuyingTheMagicalWeapon_SummaryAndCombatStatsShowIt_InThePlayersLanguage()
    {
        Player player = CreatePlayer("Thief", 100);
        Messages messages = CreateMessages("Spanish");
        Visit(player, "2\n3\n", "Spanish");

        StringWriter summary = new StringWriter();
        Console.SetOut(summary);
        player.DisplayStats(messages);

        // A Final Fight shows both sides' stats once the player attacks. With Strength 0 the
        // player misses (20), the Dragon misses (20), and with 0 Health Points the fight is lost.
        StringWriter combatStats = new StringWriter();
        Console.SetIn(new StringReader("a\n"));
        Console.SetOut(combatStats);
        FinalFightOutcome outcome = FinalFight.Run(player, new Dragon(new ScriptedDie(10, 10, 10)), messages, new ScriptedDie(20, 20));
        Assert.AreEqual(FinalFightOutcome.Lost, outcome);

        foreach (string shown in new[] { summary.ToString(), combatStats.ToString() })
        {
            StringAssert.Contains(shown, messages.GetMessage("stats_weapon") + "daga venenosa");
            StringAssert.Contains(shown, messages.GetMessage("stats_damage") + "10");
        }
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
    public void BuyingTheMagicalWeaponWith89Gold_IsRefused_AndPlayerStaysUntilLeave()
    {
        Player player = CreatePlayer("Thief", 89);
        Weapon startingWeapon = player.Weapon!;

        string output = Visit(player, "2\n3\n");

        Assert.AreEqual(89, player.Gold);
        Assert.AreSame(startingWeapon, player.Weapon);
        Assert.AreEqual("dagger", player.Weapon!.Type);
        Assert.AreEqual(6, player.Weapon.MaxDamage);
        int refusal = output.IndexOf("You don't have enough Gold for that.");
        Assert.IsTrue(refusal >= 0, "Expected the 'not enough Gold' message.");
        Assert.IsTrue(output.IndexOf("3. Leave", refusal) > refusal, "The menu should be shown again after a refusal.");
    }

    [TestMethod]
    public void PickingTheMagicalWeaponWhenAlreadyOwned_IsRefused_AndGoldDoesNotChange()
    {
        Weapon owned = OccupationEquipment.For("Fighter")!.CreateMagicalWeapon();
        Player player = new Player { Occupation = "Fighter", Gold = 100, Weapon = owned };

        string output = Visit(player, "2\n3\n");

        StringAssert.Contains(output, "You already have that.");
        Assert.AreEqual(100, player.Gold);
        Assert.AreSame(owned, player.Weapon);
    }

    [TestMethod]
    public void BuyingBothWith160Gold_LeavesNoGold_AndEndsWithNothingLeftToSell()
    {
        Player player = CreatePlayer("Magician", 160);

        // The trailing "x" would be an invalid choice if the visit carried on after both purchases.
        string output = Visit(player, "1\n2\nx\n");

        Assert.AreEqual(0, player.Gold);
        Assert.AreEqual("warded robe", player.Armor?.Name);
        Assert.AreEqual("chain lightning spell", player.Weapon?.Type);
        Assert.IsTrue(output.TrimEnd().EndsWith("'I have nothing left to sell you, friend. Safe travels!'"),
            "The visit should end with the 'nothing left to sell' message.");
        Assert.IsFalse(output.Contains("Invalid choice"), "The visit should end once nothing is left to sell.");
    }

    [TestMethod]
    public void AfterOnePurchase_TheMenuShowsTheItemLeft_UntilLeave()
    {
        Player player = CreatePlayer("Fighter", 100);

        string output = Visit(player, "2\n3\n");

        int bought = output.IndexOf("You buy a Magical Weapon (flaming sword) for 90 Gold.");
        Assert.IsTrue(bought >= 0, "Expected the purchase confirmation.");
        string afterPurchase = output.Substring(bought);
        StringAssert.Contains(afterPurchase, "You have 10 Gold.");
        StringAssert.Contains(afterPurchase, "1. Armor: enchanted plate armor (70 Gold)");
        Assert.IsFalse(afterPurchase.Contains("2. Magical Weapon:"), "The owned Magical Weapon should no longer be offered.");
        StringAssert.Contains(afterPurchase, "3. Leave");
        Assert.IsFalse(output.Contains("nothing left to sell"));
    }

    [TestMethod]
    public void OnALaterVisit_TheMagicalWeaponIsNotOfferedOnceOwned()
    {
        Player player = CreatePlayer("Thief", 100);
        Visit(player, "2\n3\n");

        string secondVisit = Visit(player, "3\n");

        Assert.IsFalse(secondVisit.Contains("2. Magical Weapon:"), "The owned Magical Weapon should not be offered again.");
        StringAssert.Contains(secondVisit, "1. Armor: shadow leathers (70 Gold)");
    }

    [TestMethod]
    public void OnALaterVisit_WithBothOwned_GoesStraightToNothingLeftToSell()
    {
        Player player = CreatePlayer("Archer", 200);
        Visit(player, "1\n2\n");

        string secondVisit = Visit(player, "x\n");

        StringAssert.Contains(secondVisit, "'I have nothing left to sell you, friend. Safe travels!'");
        Assert.IsFalse(secondVisit.Contains("The Merchant offers:"), "No menu should be shown when nothing is left.");
        Assert.IsFalse(secondVisit.Contains("Invalid choice"));
        Assert.AreEqual(40, player.Gold);
    }

    [TestMethod]
    public void TheMenuOffersTheMagicalWeapon_WithItsPrice()
    {
        Player player = CreatePlayer("Archer", 10);

        string output = Visit(player, "3\n");

        StringAssert.Contains(output, "1. Armor: elven cloak (70 Gold)");
        StringAssert.Contains(output, "2. Magical Weapon: elven bow (90 Gold)");
    }

    [TestMethod]
    [DataRow("4")]
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

    [TestMethod]
    public void BuyingTheMagicalWeapon_ConfirmsThePurchase_InThePlayersLanguage()
    {
        Player player = CreatePlayer("Fighter", 100);

        string output = Visit(player, "2\n3\n", "Italian");

        StringAssert.Contains(output, "2. Arma Magica: spada fiammeggiante (90 Oro)");
        StringAssert.Contains(output, "Compri un'Arma Magica (spada fiammeggiante) per 90 Oro.");
    }
}
