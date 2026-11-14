using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
[TestClass]
public class DragonTests
{

    private Messages GetMessages(string language = "English")
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // The dragon rolls Strength 20, Agility 1 and Health 20 when created, then its attack
    // roll hits (1), the player's dodge roll fails (20) and the damage roll is as given.
    private static Dragon CreateDragonThatHitsFor(int damageRoll)
    {
        return new Dragon(new ScriptedDie(20, 1, 20, 1, 20, damageRoll));
    }

    private static Player CreateTargetPlayer(Armor? armor, int healthPoints = 30)
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
    public void NewDragon_InitializesRequiredProperties()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Assert.AreEqual("Smolderfang", dragon.Name);
        Assert.IsTrue(dragon.Strength >= 1 && dragon.Strength <= 20);
        Assert.IsTrue(dragon.Agility >= 1 && dragon.Agility <= 20);
        Assert.IsTrue(dragon.HealthPoints >= 1 && dragon.HealthPoints <= 20);

        Assert.IsNotNull(dragon.Weapon);
        Assert.AreEqual("claws", dragon.Weapon.Type);
        Assert.AreEqual(12, dragon.Weapon.MaxDamage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(dragon.Weapon.AsciiArt));
    }

    [TestMethod]
    public void GetRandomDefenseTaunt_ReturnsExpectedTaunt()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);
        Messages messages = GetMessages();

        string taunt = dragon.GetRandomDefenseTaunt(messages);

        string[] validTaunts =
        {
        messages.GetMessage("dragon_defense_taunt_1"),
        messages.GetMessage("dragon_defense_taunt_2"),
        messages.GetMessage("dragon_defense_taunt_3"),
        messages.GetMessage("dragon_defense_taunt_4")
    };

        CollectionAssert.Contains(validTaunts, taunt);
    }

    [TestMethod]
    public void GetRandomDamageReply_ReturnsExpectedReply()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);
        Messages messages = GetMessages();

        string reply = dragon.GetRandomDamageReply(messages);

        string[] validReplies =
        {
        messages.GetMessage("dragon_damage_reply_1"),
        messages.GetMessage("dragon_damage_reply_2"),
        messages.GetMessage("dragon_damage_reply_3"),
        messages.GetMessage("dragon_damage_reply_4")
    };

        CollectionAssert.Contains(validReplies, reply);
    }

    [TestMethod]
    public void Attack_ReturnsNonEmptyResultString()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Agility = 10,
            HealthPoints = 30
        };

        string result = dragon.Attack(player, GetMessages());

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void Attack_DoesNotIncreasePlayerHealth()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Agility = 10,
            HealthPoints = 30
        };

        int startingHealth = player.HealthPoints;

        string result = dragon.Attack(player, GetMessages());

        Assert.IsTrue(player.HealthPoints <= startingHealth);
    }

    [TestMethod]
    public void Attack_KeepsPlayerHealthNonNegative()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Agility = 0,
            HealthPoints = 1
        };

        string result = dragon.Attack(player, GetMessages());

        Assert.IsTrue(player.HealthPoints >= 0);
    }

    [TestMethod]
    public void Attack_WithArmor_ReducesDamageByProtection()
    {
        Player player = CreateTargetPlayer(new Armor("shadow leathers", 3));

        string result = CreateDragonThatHitsFor(5).Attack(player, GetMessages());

        Assert.AreEqual(28, player.HealthPoints);
        StringAssert.Contains(result, "Armor (shadow leathers): 3 damage absorbed, 2 damage taken.");
        StringAssert.Contains(result, "TestHero's health points are now 28.");
    }

    [TestMethod]
    public void Attack_WithArmor_DamageEqualToProtection_AbsorbsTheWholeHit()
    {
        Player player = CreateTargetPlayer(new Armor("shadow leathers", 3));

        string result = CreateDragonThatHitsFor(3).Attack(player, GetMessages());

        Assert.AreEqual(30, player.HealthPoints);
        StringAssert.Contains(result, "Armor (shadow leathers): the whole hit is absorbed, no damage taken!");
        Assert.IsFalse(result.Contains("damage absorbed,"), "Only the whole-hit line should appear.");
    }

    [TestMethod]
    public void Attack_WithArmor_DamageBelowProtection_NeverHeals()
    {
        Player player = CreateTargetPlayer(new Armor("shadow leathers", 3));

        string result = CreateDragonThatHitsFor(2).Attack(player, GetMessages());

        Assert.AreEqual(30, player.HealthPoints);
        StringAssert.Contains(result, "Armor (shadow leathers): the whole hit is absorbed, no damage taken!");
    }

    [TestMethod]
    public void Attack_WithArmor_PlayerReachingZeroHealthIsDefeated()
    {
        Player player = CreateTargetPlayer(new Armor("shadow leathers", 3), healthPoints: 2);

        string result = CreateDragonThatHitsFor(5).Attack(player, GetMessages());

        Assert.AreEqual(0, player.HealthPoints);
        StringAssert.Contains(result, "TestHero has been defeated.");
    }

    [TestMethod]
    public void Attack_WithoutArmor_DamageAndOutputAreUnchanged()
    {
        Player player = CreateTargetPlayer(armor: null);

        string result = CreateDragonThatHitsFor(5).Attack(player, GetMessages());

        Assert.AreEqual(25, player.HealthPoints);
        string expected = string.Join(Environment.NewLine,
            "Smolderfang attacks TestHero with claws!",
            "<<< claws >>>",
            "Smolderfang hits TestHero.",
            "Smolderfang deals 5 damage.",
            "TestHero's health points are now 25.");
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Attack_WithArmor_SuccessfulDodgeStillDependsOnlyOnAgility()
    {
        // Attack roll hits (1), then the dodge roll of 10 equals the player's Agility of 10.
        Dragon dragon = new Dragon(new ScriptedDie(20, 1, 20, 1, 10));
        Player player = CreateTargetPlayer(new Armor("shadow leathers", 3));

        string result = dragon.Attack(player, GetMessages());

        Assert.AreEqual(30, player.HealthPoints);
        StringAssert.Contains(result, "TestHero successfully defends the attack.");
        Assert.IsFalse(result.Contains("Armor ("), "Armor plays no part in a dodged attack.");
    }

    [TestMethod]
    [DataRow("Spanish", "Armadura (cueros de las sombras): el golpe entero es absorbido, ningun dano recibido!")]
    [DataRow("French", "Armure (cuirs des ombres): le coup entier est absorbe, aucun degat subi!")]
    [DataRow("Italian", "Armatura (cuoio delle ombre): il colpo intero viene assorbito, nessun danno subito!")]
    public void Attack_WithArmor_WholeHitMessageIsTranslated(string language, string expected)
    {
        Player player = CreateTargetPlayer(new Armor("shadow leathers", 3));

        string result = CreateDragonThatHitsFor(3).Attack(player, GetMessages(language));

        StringAssert.Contains(result, expected);
    }

    [TestMethod]
    [DataRow("Spanish", "Armadura (cueros de las sombras): 3 de dano absorbido, 2 de dano recibido.")]
    [DataRow("French", "Armure (cuirs des ombres): 3 degats absorbes, 2 degats subis.")]
    [DataRow("Italian", "Armatura (cuoio delle ombre): 3 danni assorbiti, 2 danni subiti.")]
    public void Attack_WithArmor_AbsorbedMessageIsTranslated(string language, string expected)
    {
        Player player = CreateTargetPlayer(new Armor("shadow leathers", 3));

        string result = CreateDragonThatHitsFor(5).Attack(player, GetMessages(language));

        StringAssert.Contains(result, expected);
    }
}

[TestClass]
[DoNotParallelize]
public class GamePbi18Tests
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
    public void DragonDisplayStats_UsesLocalizedLabels()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        dragon.DisplayStats(messages);

        string output = writer.ToString();

        StringAssert.Contains(output, messages.GetMessage("dragon_stats_header"));
        StringAssert.Contains(output, messages.GetMessage("stats_name"));
        StringAssert.Contains(output, messages.GetMessage("stats_strength"));
        StringAssert.Contains(output, messages.GetMessage("stats_agility"));
        StringAssert.Contains(output, messages.GetMessage("stats_health"));
        StringAssert.Contains(output, messages.GetMessage("stats_weapon"));
        StringAssert.Contains(output, messages.GetMessage("stats_damage"));
    }

    [TestMethod]
    public void FinalFight_DragonAttackPath_DisplaysEncounterFlowText()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 20,
            Agility = 20,
            HealthPoints = 25,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };

        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        // "a" starts the fight, then "r" retreats after the first Round
        Console.SetIn(new StringReader("a\nr\n"));

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        FinalFight.Run(player, dragon, messages, die);

        string output = writer.ToString();

        StringAssert.Contains(output, messages.GetMessage("dragon_path_narrative"));
        StringAssert.Contains(output, string.Format(messages.GetMessage("dragon_intro"), dragon.Name));
        StringAssert.Contains(output, messages.GetMessage("dragon_stats_intro"));
        StringAssert.Contains(output, messages.GetMessage("encounter_prompt"));
    }

    [TestMethod]
    public void DragonEncounter_MessageKeys_AreLoaded()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        Assert.AreNotEqual("[dragon_path_narrative]", messages.GetMessage("dragon_path_narrative"));
        Assert.AreNotEqual("[dragon_intro]", messages.GetMessage("dragon_intro"));
        Assert.AreNotEqual("[dragon_stats_intro]", messages.GetMessage("dragon_stats_intro"));
        Assert.AreNotEqual("[encounter_prompt]", messages.GetMessage("encounter_prompt"));
        Assert.AreNotEqual("[dragon_stats_header]", messages.GetMessage("dragon_stats_header"));
    }
}
