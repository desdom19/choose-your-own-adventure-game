using static ScriptedFinalFight;

// The Goblin: a slippery Monster that dodges more often than the Dragon but hurts less.
[TestClass]
[DoNotParallelize]
public class GoblinTests
{
    [TestMethod]
    public void NewGoblin_LowestRolls_GiveAgilitySix()
    {
        // Strength d20, Agility 5 + d15, Health Points d20.
        Goblin goblin = new Goblin(new ScriptedDie(1, 1, 1));

        Assert.AreEqual(1, goblin.Strength);
        Assert.AreEqual(6, goblin.Agility);
        Assert.AreEqual(1, goblin.HealthPoints);
    }

    [TestMethod]
    public void NewGoblin_HighestRolls_GiveAgilityTwenty()
    {
        Goblin goblin = new Goblin(new ScriptedDie(20, 15, 20));

        Assert.AreEqual(20, goblin.Strength);
        Assert.AreEqual(20, goblin.Agility);
        Assert.AreEqual(20, goblin.HealthPoints);
    }

    [TestMethod]
    public void NewGoblin_WithRealDie_StatsFallInTheirRanges()
    {
        for (int i = 0; i < 200; i++)
        {
            Goblin goblin = new Goblin(new Die());

            Assert.IsTrue(goblin.Strength >= 1 && goblin.Strength <= 20, $"Strength {goblin.Strength}");
            Assert.IsTrue(goblin.Agility >= 6 && goblin.Agility <= 20, $"Agility {goblin.Agility}");
            Assert.IsTrue(goblin.HealthPoints >= 1 && goblin.HealthPoints <= 20, $"Health Points {goblin.HealthPoints}");
        }
    }

    [TestMethod]
    public void NewGoblin_HasItsNameAndRustyCleaver()
    {
        Goblin goblin = new Goblin(new ScriptedDie(10, 10, 10));

        Assert.AreEqual("Gritknuckle", goblin.Name);
        Assert.AreEqual("rusty cleaver", goblin.Weapon.Type);
        Assert.AreEqual(9, goblin.Weapon.MaxDamage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(goblin.Weapon.AsciiArt));
        Assert.AreNotEqual(new Dragon(new ScriptedDie(1, 1, 1)).Weapon.AsciiArt, goblin.Weapon.AsciiArt);
    }

    [TestMethod]
    [DataRow("English", "rusty cleaver")]
    [DataRow("Spanish", "cuchilla oxidada")]
    [DataRow("French", "couperet rouille")]
    [DataRow("Italian", "mannaia arrugginita")]
    public void RustyCleaver_IsTranslatedForDisplay(string language, string expected)
    {
        Assert.AreEqual(expected, GetMessages(language).TranslateWeaponForDisplay("rusty cleaver"));
    }

    // Strength 10, Agility 5 + 5, Health Points 10.
    private static Goblin CreateGoblin()
    {
        return new Goblin(new ScriptedDie(10, 5, 10));
    }

    // A Thief with Strength 10, so an attack roll of 20 always misses.
    private static Player CreatePlayer(Armor? armor = null, int healthPoints = 30, string race = "Human")
    {
        return new Player
        {
            Name = "TestHero",
            Race = race,
            Occupation = "Thief",
            Strength = 10,
            Agility = 10,
            HealthPoints = healthPoints,
            Weapon = new Weapon("dagger", 6, "--|=>"),
            Armor = armor
        };
    }

    // One Round in which the player misses (20), then the Goblin strikes with the given rolls:
    // its attack roll, the player's dodge roll and the damage roll. The player retreats if
    // still standing.
    private static FinalFightOutcome GoblinStrikes(Player player, out string output, params int[] strikeRolls)
    {
        return Play("a\nr\n", player, CreateGoblin(), GetMessages(), [20, .. strikeRolls], out output);
    }

    [TestMethod]
    [DataRow(1, "Too slow! Too slow!")]
    [DataRow(2, "Ha! You swing like a sleepy troll!")]
    [DataRow(3, "Over here, Dwarf! No, over here! Hee hee!")]
    [DataRow(4, "Can't hit what you can't catch!")]
    public void GoblinDodges_PicksFromTheGoblinsOwnTaunts(int roll, string expected)
    {
        // The player hits (1), the Goblin dodges (10) and picks a taunt; then it misses (11).
        Messages messages = GetMessages();

        Play("a\nr\n", CreatePlayer(race: "Dwarf"), CreateGoblin(), messages, [1, 10, roll, 11], out string output);

        StringAssert.Contains(output, Lines("Gritknuckle successfully defends your attack.", expected, "Gritknuckle attacks TestHero with rusty cleaver!"));
        foreach (string dragonTaunt in TextsForKeys(messages, "dragon_defense_taunt_", 4))
            Assert.IsFalse(output.Contains(string.Format(dragonTaunt, "Dwarf")), $"Dragon taunt shown: {dragonTaunt}");
    }

    [TestMethod]
    [DataRow(1, "Ouch! No fair, no fair!")]
    [DataRow(2, "You'll pay for that, you big lump!")]
    [DataRow(3, "Yowch! That's going to leave a mark!")]
    [DataRow(4, "Lucky hit! Won't happen twice!")]
    public void GoblinTakesDamage_PicksFromTheGoblinsOwnReplies(int roll, string expected)
    {
        // The player hits (1), the dodge fails (20), 2 damage, then a reply; the Goblin misses (11).
        Messages messages = GetMessages();

        Play("a\nr\n", CreatePlayer(), CreateGoblin(), messages, [1, 20, 2, roll, 11], out string output);

        StringAssert.Contains(output, Lines("Gritknuckle's health points are now 8.", expected, "Gritknuckle attacks TestHero with rusty cleaver!"));
        foreach (string dragonReply in TextsForKeys(messages, "dragon_damage_reply_", 4))
            Assert.IsFalse(output.Contains(dragonReply), $"Dragon reply shown: {dragonReply}");
    }

    [TestMethod]
    public void Attack_Hit_PrintsExactText()
    {
        Player player = CreatePlayer();

        GoblinStrikes(player, out string result, 1, 20, 6);

        Assert.AreEqual(24, player.HealthPoints);
        StringAssert.Contains(result, Lines(
            "You missed Gritknuckle.",
            "Gritknuckle attacks TestHero with rusty cleaver!",
            "|=[####]",
            "Gritknuckle hits TestHero.",
            "Gritknuckle deals 6 damage.",
            "TestHero's health points are now 24.",
            "Enter 'a' to attack or 'r' to retreat."));
    }

    [TestMethod]
    public void Attack_Miss_PrintsExactText()
    {
        Player player = CreatePlayer();

        // An attack roll of 11 is above the Goblin's Strength of 10.
        GoblinStrikes(player, out string result, 11);

        Assert.AreEqual(30, player.HealthPoints);
        StringAssert.Contains(result, Lines(
            "You missed Gritknuckle.",
            "Gritknuckle attacks TestHero with rusty cleaver!",
            "|=[####]",
            "Gritknuckle missed TestHero.",
            "Enter 'a' to attack or 'r' to retreat."));
    }

    [TestMethod]
    public void Attack_PlayerDodges_PrintsExactText()
    {
        Player player = CreatePlayer(new Armor("shadow leathers", 3));

        // The dodge roll of 10 equals the player's Agility of 10.
        GoblinStrikes(player, out string result, 1, 10);

        Assert.AreEqual(30, player.HealthPoints);
        StringAssert.Contains(result, Lines(
            "You missed Gritknuckle.",
            "Gritknuckle attacks TestHero with rusty cleaver!",
            "|=[####]",
            "Gritknuckle hits TestHero.",
            "TestHero successfully defends the attack.",
            "Enter 'a' to attack or 'r' to retreat."));
    }

    [TestMethod]
    public void Attack_WithArmor_PartialHit_ReducesDamageByProtection()
    {
        Player player = CreatePlayer(new Armor("shadow leathers", 3));

        GoblinStrikes(player, out string result, 1, 20, 8);

        Assert.AreEqual(25, player.HealthPoints);
        StringAssert.Contains(result, "Gritknuckle deals 8 damage.");
        StringAssert.Contains(result, "Armor (shadow leathers): 3 damage absorbed, 5 damage taken.");
        StringAssert.Contains(result, "TestHero's health points are now 25.");
    }

    [TestMethod]
    public void Attack_WithArmor_WholeHit_IsAbsorbed()
    {
        Player player = CreatePlayer(new Armor("shadow leathers", 3));

        GoblinStrikes(player, out string result, 1, 20, 2);

        Assert.AreEqual(30, player.HealthPoints);
        StringAssert.Contains(result, "Armor (shadow leathers): the whole hit is absorbed, no damage taken!");
        Assert.IsFalse(result.Contains("damage absorbed,"), "Only the whole-hit line should appear.");
    }

    [TestMethod]
    public void Attack_HealthPointsNeverDropBelowZero()
    {
        Player player = CreatePlayer(healthPoints: 3);

        FinalFightOutcome outcome = GoblinStrikes(player, out string result, 1, 20, 8);

        Assert.AreEqual(FinalFightOutcome.Lost, outcome);
        Assert.AreEqual(0, player.HealthPoints);
        StringAssert.Contains(result, "TestHero's health points are now 0.");
        StringAssert.Contains(result, "TestHero has been defeated.");
    }

    [TestMethod]
    public void PlayerAttack_GoblinDodges_TauntsWithThePlayersRace()
    {
        // The Goblin's Agility is 5 + 5 = 10; it dodges a roll of 10 and picks taunt 3.
        // Then it misses (11).
        Goblin goblin = CreateGoblin();
        Player player = CreatePlayer(race: "Dwarf");

        Play("a\nr\n", player, goblin, GetMessages(), [1, 10, 3, 11], out string result);

        Assert.AreEqual(10, goblin.HealthPoints);
        StringAssert.Contains(result, Lines(
            "As a Thief, you withdraw your dagger and attack the goblin.",
            "--|=>",
            "You hit Gritknuckle.",
            "Gritknuckle successfully defends your attack.",
            "Over here, Dwarf! No, over here! Hee hee!",
            "Gritknuckle attacks TestHero with rusty cleaver!"));
    }

    [TestMethod]
    public void PlayerAttack_DefenseRollOfTwenty_LandsEvenAgainstAgilityTwenty()
    {
        // The Goblin rolls its highest Agility, 5 + 15 = 20. A defense roll of 20 never
        // dodges, so a fight against it can always be won. The player hits (1) for 4, the
        // Goblin replies (1), then misses (11).
        Goblin goblin = new Goblin(new ScriptedDie(10, 15, 10));

        Play("a\nr\n", CreatePlayer(), goblin, GetMessages(), [1, 20, 4, 1, 11], out string result);

        Assert.AreEqual(6, goblin.HealthPoints);
        StringAssert.Contains(result, "Gritknuckle's health points are now 6.");
    }

    [TestMethod]
    public void Attack_DefenseRollOfTwenty_LandsEvenAgainstPlayerAgilityAboveTwenty()
    {
        // An Elf can reach Agility 24. A defense roll of 20 still never dodges.
        Player player = CreatePlayer();
        player.Agility = 24;

        GoblinStrikes(player, out string result, 1, 20, 6);

        Assert.AreEqual(24, player.HealthPoints);
        StringAssert.Contains(result, "Gritknuckle deals 6 damage.");
    }

    private static string[] TextsForKeys(Messages messages, string keyPrefix, int count)
    {
        return Enumerable.Range(1, count).Select(i => messages.GetMessage(keyPrefix + i)).ToArray();
    }
}
