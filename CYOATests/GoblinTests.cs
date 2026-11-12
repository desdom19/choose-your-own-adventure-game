// The Goblin: a slippery Monster that dodges more often than the Dragon but hurts less.
[TestClass]
public class GoblinTests
{
    private static Messages GetMessages(string language = "English")
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

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

    [TestMethod]
    [DataRow(1, "Too slow! Too slow!")]
    [DataRow(2, "Ha! You swing like a sleepy troll!")]
    [DataRow(3, "Over here, {0}! No, over here! Hee hee!")]
    [DataRow(4, "Can't hit what you can't catch!")]
    public void GetRandomDefenseTaunt_PicksFromTheGoblinsOwnTaunts(int roll, string expected)
    {
        Goblin goblin = new Goblin(new ScriptedDie(10, 10, 10, roll));
        Messages messages = GetMessages();

        string taunt = goblin.GetRandomDefenseTaunt(messages);

        Assert.AreEqual(expected, taunt);
        CollectionAssert.DoesNotContain(TextsForKeys(messages, "dragon_defense_taunt_", 4), taunt);
    }

    [TestMethod]
    [DataRow(1, "Ouch! No fair, no fair!")]
    [DataRow(2, "You'll pay for that, you big lump!")]
    [DataRow(3, "Yowch! That's going to leave a mark!")]
    [DataRow(4, "Lucky hit! Won't happen twice!")]
    public void GetRandomDamageReply_PicksFromTheGoblinsOwnReplies(int roll, string expected)
    {
        Goblin goblin = new Goblin(new ScriptedDie(10, 10, 10, roll));
        Messages messages = GetMessages();

        string reply = goblin.GetRandomDamageReply(messages);

        Assert.AreEqual(expected, reply);
        CollectionAssert.DoesNotContain(TextsForKeys(messages, "dragon_damage_reply_", 4), reply);
    }

    // The Goblin rolls Strength 10, Agility 5 + 5 and Health 10 when created; the rolls
    // that follow are its attack roll, the player's dodge roll and the damage roll.
    private static Goblin CreateGoblin(params int[] attackRolls)
    {
        return new Goblin(new ScriptedDie(new[] { 10, 5, 10 }.Concat(attackRolls).ToArray()));
    }

    private static Player CreateTargetPlayer(Armor? armor = null, int healthPoints = 30)
    {
        return new Player
        {
            Name = "TestHero",
            Agility = 10,
            HealthPoints = healthPoints,
            Armor = armor
        };
    }

    [TestMethod]
    public void Attack_Hit_PrintsExactText()
    {
        Player player = CreateTargetPlayer();

        string result = CreateGoblin(1, 20, 6).Attack(player, GetMessages());

        Assert.AreEqual(24, player.HealthPoints);
        Assert.AreEqual(string.Join(Environment.NewLine,
            "Gritknuckle attacks TestHero with rusty cleaver!",
            "|=[####]",
            "Gritknuckle hits TestHero.",
            "Gritknuckle deals 6 damage.",
            "TestHero's health points are now 24."), result);
    }

    [TestMethod]
    public void Attack_Miss_PrintsExactText()
    {
        Player player = CreateTargetPlayer();

        // An attack roll of 11 is above the Goblin's Strength of 10.
        string result = CreateGoblin(11).Attack(player, GetMessages());

        Assert.AreEqual(30, player.HealthPoints);
        Assert.AreEqual(string.Join(Environment.NewLine,
            "Gritknuckle attacks TestHero with rusty cleaver!",
            "|=[####]",
            "Gritknuckle missed TestHero."), result);
    }

    [TestMethod]
    public void Attack_PlayerDodges_PrintsExactText()
    {
        Player player = CreateTargetPlayer(new Armor("shadow leathers", 3));

        // The dodge roll of 10 equals the player's Agility of 10.
        string result = CreateGoblin(1, 10).Attack(player, GetMessages());

        Assert.AreEqual(30, player.HealthPoints);
        Assert.AreEqual(string.Join(Environment.NewLine,
            "Gritknuckle attacks TestHero with rusty cleaver!",
            "|=[####]",
            "Gritknuckle hits TestHero.",
            "TestHero successfully defends the attack."), result);
    }

    [TestMethod]
    public void Attack_WithArmor_PartialHit_ReducesDamageByProtection()
    {
        Player player = CreateTargetPlayer(new Armor("shadow leathers", 3));

        string result = CreateGoblin(1, 20, 8).Attack(player, GetMessages());

        Assert.AreEqual(25, player.HealthPoints);
        StringAssert.Contains(result, "Gritknuckle deals 8 damage.");
        StringAssert.Contains(result, "Armor (shadow leathers): 3 damage absorbed, 5 damage taken.");
        StringAssert.Contains(result, "TestHero's health points are now 25.");
    }

    [TestMethod]
    public void Attack_WithArmor_WholeHit_IsAbsorbed()
    {
        Player player = CreateTargetPlayer(new Armor("shadow leathers", 3));

        string result = CreateGoblin(1, 20, 2).Attack(player, GetMessages());

        Assert.AreEqual(30, player.HealthPoints);
        StringAssert.Contains(result, "Armor (shadow leathers): the whole hit is absorbed, no damage taken!");
        Assert.IsFalse(result.Contains("damage absorbed,"), "Only the whole-hit line should appear.");
    }

    [TestMethod]
    public void Attack_HealthPointsNeverDropBelowZero()
    {
        Player player = CreateTargetPlayer(healthPoints: 3);

        string result = CreateGoblin(1, 20, 8).Attack(player, GetMessages());

        Assert.AreEqual(0, player.HealthPoints);
        StringAssert.Contains(result, "TestHero's health points are now 0.");
        StringAssert.Contains(result, "TestHero has been defeated.");
    }

    [TestMethod]
    public void PlayerAttack_GoblinDodges_TauntsWithThePlayersRace()
    {
        // The Goblin's Agility is 5 + 5 = 10; it dodges a roll of 10 and picks taunt 3.
        Goblin goblin = CreateGoblin(3);
        Player player = new Player(new ScriptedDie(1, 10))
        {
            Race = "Dwarf",
            Occupation = "Thief",
            Strength = 15,
            Weapon = new Weapon("dagger", 6, "--|=>")
        };

        string result = player.Attack(goblin, GetMessages());

        Assert.AreEqual(10, goblin.HealthPoints);
        Assert.AreEqual(string.Join(Environment.NewLine,
            "As a Thief, you withdraw your dagger and attack the goblin.",
            "--|=>",
            "You hit Gritknuckle.",
            "Gritknuckle successfully defends your attack.",
            "Over here, Dwarf! No, over here! Hee hee!"), result);
    }

    [TestMethod]
    public void PlayerAttack_DefenseRollOfTwenty_LandsEvenAgainstAgilityTwenty()
    {
        // The Goblin rolls its highest Agility, 5 + 15 = 20. A defense roll of 20 never
        // dodges, so a fight against it can always be won.
        Goblin goblin = new Goblin(new ScriptedDie(10, 15, 10, 1));
        Player player = new Player(new ScriptedDie(1, 20, 4))
        {
            Race = "Human",
            Occupation = "Thief",
            Strength = 15,
            Weapon = new Weapon("dagger", 6, "--|=>")
        };

        string result = player.Attack(goblin, GetMessages());

        Assert.AreEqual(6, goblin.HealthPoints);
        StringAssert.Contains(result, "Gritknuckle's health points are now 6.");
    }

    [TestMethod]
    public void Attack_DefenseRollOfTwenty_LandsEvenAgainstPlayerAgilityAboveTwenty()
    {
        // An Elf can reach Agility 24. A defense roll of 20 still never dodges.
        Player player = CreateTargetPlayer();
        player.Agility = 24;

        string result = CreateGoblin(1, 20, 6).Attack(player, GetMessages());

        Assert.AreEqual(24, player.HealthPoints);
        StringAssert.Contains(result, "Gritknuckle deals 6 damage.");
    }

    private static string[] TextsForKeys(Messages messages, string keyPrefix, int count)
    {
        return Enumerable.Range(1, count).Select(i => messages.GetMessage(keyPrefix + i)).ToArray();
    }
}
