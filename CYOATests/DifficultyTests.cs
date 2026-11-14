// Difficulty changes how strong a Monster is: its rolled stats and its weapon's maximum
// damage, by the same amount for every Monster, within limits that keep it able to fight.
[TestClass]
public class DifficultyTests
{
    // --- Dragon: Strength d20, Agility d20, Health Points d20, claws 12 ---

    [TestMethod]
    public void NormalDragon_GetsTheRolledStatsAndClawsOfTwelve()
    {
        Dragon dragon = new Dragon(new ScriptedDie(10, 11, 12), Difficulty.Normal);

        Assert.AreEqual(10, dragon.Strength);
        Assert.AreEqual(11, dragon.Agility);
        Assert.AreEqual(12, dragon.HealthPoints);
        Assert.AreEqual(12, dragon.Weapon.MaxDamage);
    }

    [TestMethod]
    public void HardDragon_GetsThreeMoreOnEveryStat()
    {
        Dragon dragon = new Dragon(new ScriptedDie(10, 11, 12), Difficulty.Hard);

        Assert.AreEqual(13, dragon.Strength);
        Assert.AreEqual(14, dragon.Agility);
        Assert.AreEqual(15, dragon.HealthPoints);
        Assert.AreEqual(15, dragon.Weapon.MaxDamage);
    }

    [TestMethod]
    public void EasyDragon_GetsThreeLessOnEveryStat()
    {
        Dragon dragon = new Dragon(new ScriptedDie(10, 11, 12), Difficulty.Easy);

        Assert.AreEqual(7, dragon.Strength);
        Assert.AreEqual(8, dragon.Agility);
        Assert.AreEqual(9, dragon.HealthPoints);
        Assert.AreEqual(9, dragon.Weapon.MaxDamage);
    }

    [TestMethod]
    public void EasyDragon_LowestRolls_StopAtOne()
    {
        Dragon dragon = new Dragon(new ScriptedDie(1, 1, 1), Difficulty.Easy);

        Assert.AreEqual(1, dragon.Strength);
        Assert.AreEqual(1, dragon.Agility);
        Assert.AreEqual(1, dragon.HealthPoints);
    }

    [TestMethod]
    public void HardDragon_HighestRolls_StopStrengthAndAgilityAtTwenty_ButNotHealthPoints()
    {
        Dragon dragon = new Dragon(new ScriptedDie(20, 20, 20), Difficulty.Hard);

        Assert.AreEqual(20, dragon.Strength);
        Assert.AreEqual(20, dragon.Agility);
        Assert.AreEqual(23, dragon.HealthPoints);
    }

    [TestMethod]
    public void DragonWithoutADifficulty_IsTheSameAsNormal()
    {
        Dragon withoutDifficulty = new Dragon(new ScriptedDie(4, 17, 9));
        Dragon normal = new Dragon(new ScriptedDie(4, 17, 9), Difficulty.Normal);

        Assert.AreEqual(normal.Strength, withoutDifficulty.Strength);
        Assert.AreEqual(normal.Agility, withoutDifficulty.Agility);
        Assert.AreEqual(normal.HealthPoints, withoutDifficulty.HealthPoints);
        Assert.AreEqual(normal.Weapon.MaxDamage, withoutDifficulty.Weapon.MaxDamage);
    }

    // --- Goblin: Strength d20, Agility 5 + d15, Health Points d20, rusty cleaver 9 ---

    [TestMethod]
    public void NormalGoblin_GetsTheRolledStatsAndCleaverOfNine()
    {
        Goblin goblin = new Goblin(new ScriptedDie(10, 6, 12), Difficulty.Normal);

        Assert.AreEqual(10, goblin.Strength);
        Assert.AreEqual(11, goblin.Agility);
        Assert.AreEqual(12, goblin.HealthPoints);
        Assert.AreEqual(9, goblin.Weapon.MaxDamage);
    }

    [TestMethod]
    public void HardGoblin_GetsThreeMoreOnEveryStat()
    {
        Goblin goblin = new Goblin(new ScriptedDie(10, 6, 12), Difficulty.Hard);

        Assert.AreEqual(13, goblin.Strength);
        Assert.AreEqual(14, goblin.Agility);
        Assert.AreEqual(15, goblin.HealthPoints);
        Assert.AreEqual(12, goblin.Weapon.MaxDamage);
    }

    [TestMethod]
    public void EasyGoblin_GetsThreeLessOnEveryStat()
    {
        Goblin goblin = new Goblin(new ScriptedDie(10, 6, 12), Difficulty.Easy);

        Assert.AreEqual(7, goblin.Strength);
        Assert.AreEqual(8, goblin.Agility);
        Assert.AreEqual(9, goblin.HealthPoints);
        Assert.AreEqual(6, goblin.Weapon.MaxDamage);
    }

    [TestMethod]
    public void EasyGoblin_LowestRolls_StopAtOne_AndAgilityDropsToThree()
    {
        Goblin goblin = new Goblin(new ScriptedDie(1, 1, 1), Difficulty.Easy);

        Assert.AreEqual(1, goblin.Strength);
        Assert.AreEqual(3, goblin.Agility);
        Assert.AreEqual(1, goblin.HealthPoints);
    }

    [TestMethod]
    public void HardGoblin_HighestRolls_StopStrengthAndAgilityAtTwenty_ButNotHealthPoints()
    {
        Goblin goblin = new Goblin(new ScriptedDie(20, 15, 20), Difficulty.Hard);

        Assert.AreEqual(20, goblin.Strength);
        Assert.AreEqual(20, goblin.Agility);
        Assert.AreEqual(23, goblin.HealthPoints);
    }

    [TestMethod]
    public void GoblinWithoutADifficulty_IsTheSameAsNormal()
    {
        Goblin withoutDifficulty = new Goblin(new ScriptedDie(4, 13, 9));
        Goblin normal = new Goblin(new ScriptedDie(4, 13, 9), Difficulty.Normal);

        Assert.AreEqual(normal.Strength, withoutDifficulty.Strength);
        Assert.AreEqual(normal.Agility, withoutDifficulty.Agility);
        Assert.AreEqual(normal.HealthPoints, withoutDifficulty.HealthPoints);
        Assert.AreEqual(normal.Weapon.MaxDamage, withoutDifficulty.Weapon.MaxDamage);
    }

    // --- Every Monster ---

    // Every Difficulty uses exactly three rolls to build a Monster, so the fourth scripted
    // roll is always the Monster's first attack roll.
    [TestMethod]
    [DataRow("Easy")]
    [DataRow("Normal")]
    [DataRow("Hard")]
    public void EveryDifficulty_UsesExactlyThreeRollsToBuildAMonster(string difficultyName)
    {
        Difficulty difficulty = DifficultyNamed(difficultyName);

        Assert.AreEqual(7, new Dragon(new ScriptedDie(10, 10, 10, 7), difficulty).RollAttack());
        Assert.AreEqual(7, new Goblin(new ScriptedDie(10, 10, 10, 7), difficulty).RollAttack());
    }

    // Building a Hard Dragon and an Easy Goblin first must not change the weapons of the
    // Monsters built after them.
    [TestMethod]
    public void Difficulty_ChangesOnlyTheMonstersOwnWeapon()
    {
        new Dragon(new ScriptedDie(10, 10, 10), Difficulty.Hard);
        new Goblin(new ScriptedDie(10, 10, 10), Difficulty.Easy);

        Assert.AreEqual(12, new Dragon(new ScriptedDie(10, 10, 10)).Weapon.MaxDamage);
        Assert.AreEqual(9, new Goblin(new ScriptedDie(10, 10, 10)).Weapon.MaxDamage);
    }

    private static Difficulty DifficultyNamed(string name) => name switch
    {
        "Easy" => Difficulty.Easy,
        "Normal" => Difficulty.Normal,
        "Hard" => Difficulty.Hard,
        _ => throw new ArgumentException($"No Difficulty named {name}.")
    };
}

// A whole Final Fight on Hard: the combat stats show the stats after Difficulty, and the
// fight is decided by them.
[TestClass]
[DoNotParallelize]
public class DifficultyCombatTests
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
    public void HardGoblin_FightShowsAndUsesTheStatsAfterDifficulty()
    {
        // Rolled Strength 10, Agility 5 + 5, Health Points 6; on Hard that's 13, 13, 9 and
        // a cleaver of 12. Round 1: the Goblin dodges a defense roll of 13 and taunts (1),
        // then hits with an attack roll of 13, the dodge fails (20) and it deals 12, which
        // only a cleaver of 12 can roll. Round 2: the player hits for 9, exactly the
        // Goblin's 9 Health Points.
        Goblin goblin = new Goblin(new ScriptedDie(10, 5, 6, 1, 13, 20, 12), Difficulty.Hard);
        OccupationEquipment equipment = OccupationEquipment.For("Fighter")!;
        Player player = new Player(new ScriptedDie(1, 13, 1, 20, 9))
        {
            Name = "TestHero",
            Race = "Elf",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = 20,
            Weapon = equipment.CreateMagicalWeapon(),
            Armor = equipment.CreateArmor()
        };
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        // "a" at the encounter prompt, then "a" again after Round 1.
        Console.SetIn(new StringReader("a\na\n"));
        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        FinalFightOutcome outcome = FinalFight.Run(player, goblin, messages, new ScriptedDie());

        // The Rounds, from the stats before the first one, come just before the victory ending.
        Assert.AreEqual(FinalFightOutcome.Won, outcome);
        string output = writer.ToString().Replace("\r\n", "\n").TrimEnd();
        string victory = "With the goblin beaten, the west road is safe again. You return home a hero. The End.";
        StringAssert.EndsWith(output, "\n" + victory);
        StringAssert.EndsWith(output[..^victory.Length].TrimEnd(), """
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
            Strength: 13
            Agility: 13
            Health Points: 9
            Weapon: rusty cleaver
            Weapon's Maximum Damage Points: 12
            As a Fighter, you withdraw your flaming sword and attack the goblin.
            -)=====>~*~*
            You hit Gritknuckle.
            Gritknuckle successfully defends your attack.
            Too slow! Too slow!
            Gritknuckle attacks TestHero with rusty cleaver!
            |=[####]
            Gritknuckle hits TestHero.
            Gritknuckle deals 12 damage.
            Armor (enchanted plate armor): 3 damage absorbed, 9 damage taken.
            TestHero's health points are now 11.
            Enter 'a' to attack or 'r' to retreat.
            As a Fighter, you withdraw your flaming sword and attack the goblin.
            -)=====>~*~*
            You hit Gritknuckle.
            You deal 9 damage.
            Gritknuckle's health points are now 0.
            With a last squeal of 'Eeek!', you have defeated the goblin.
            """.Replace("\r\n", "\n"));
    }
}
