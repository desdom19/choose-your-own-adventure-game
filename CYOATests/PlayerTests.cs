using static ScriptedFinalFight;

// The player, and the player's Strike against a Monster through short scripted Final Fights.
[TestClass]
[DoNotParallelize]
public class PlayerTests
{
    [TestMethod]
    public void NewPlayer_ShouldHaveDefaultValues()
    {
        Player player = new Player();

        Assert.AreEqual("", player.Name);
        Assert.AreEqual("", player.Race);
        Assert.AreEqual("", player.Occupation);

        Assert.AreEqual(0, player.Strength);
        Assert.AreEqual(0, player.Agility);
        Assert.AreEqual(0, player.HealthPoints);

        Assert.IsNull(player.Weapon);
    }

    // A Fighter with Strength 10 and a long sword (d12).
    private static Player CreatePlayer()
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 10,
            HealthPoints = 20,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    // Strength 10, Agility 10 and the given Health Points.
    private static Dragon CreateDragon(int healthPoints)
    {
        return new Dragon(new ScriptedDie(10, 10, healthPoints));
    }

    [TestMethod]
    public void Attack_WithForcedRolls_DealsExactDamageToDragon()
    {
        // Attack roll 5 hits (Strength 10), defense roll 15 beats the Dragon's Agility 10,
        // damage roll 7 on the long sword's d12, then reply 1. The Dragon misses (20).
        Dragon dragon = CreateDragon(20);

        Play("a\nr\n", CreatePlayer(), dragon, GetMessages(), [5, 15, 7, 1, 20], out string output);

        Assert.AreEqual(13, dragon.HealthPoints);
        StringAssert.Contains(output, Lines(
            "As a Fighter, you withdraw your long sword and attack the dragon.",
            "-)=====>",
            "You hit Smolderfang.",
            "You deal 7 damage.",
            "Smolderfang's health points are now 13.",
            "Ow! That actually stung a bit!"));
    }

    [TestMethod]
    public void Attack_Miss_PrintsExactText()
    {
        // An attack roll of 11 is above the player's Strength of 10. The Dragon misses (20).
        Dragon dragon = CreateDragon(20);

        Play("a\nr\n", CreatePlayer(), dragon, GetMessages(), [11, 20], out string output);

        Assert.AreEqual(20, dragon.HealthPoints);
        StringAssert.Contains(output, Lines(
            "As a Fighter, you withdraw your long sword and attack the dragon.",
            "-)=====>",
            "You missed Smolderfang.",
            "Smolderfang attacks TestHero with claws!"));
    }

    [TestMethod]
    public void Attack_BringingTheDragonToZero_DefeatsIt()
    {
        // The player hits (5), the dodge fails (15) and deals 5, all the Dragon's Health Points.
        Dragon dragon = CreateDragon(5);

        FinalFightOutcome outcome = Play("a\n", CreatePlayer(), dragon, GetMessages(), [5, 15, 5], out string output);

        Assert.AreEqual(FinalFightOutcome.Won, outcome);
        Assert.AreEqual(0, dragon.HealthPoints);
        StringAssert.Contains(output, Lines(
            "You deal 5 damage.",
            "Smolderfang's health points are now 0.",
            "With the last words of 'Arggg!', you have defeated the dragon."));
    }

    [TestMethod]
    [DataRow("English", "You have no weapon to attack with.")]
    [DataRow("Spanish", "No tienes ningun arma con la que atacar.")]
    [DataRow("French", "Vous n'avez aucune arme pour attaquer.")]
    [DataRow("Italian", "Non hai nessuna arma con cui attaccare.")]
    public void Attack_WithoutWeapon_SaysSoInTheChosenLanguage(string language, string expected)
    {
        // Without a weapon the player can't strike. The Dragon then hits (1), the dodge
        // fails (20) and its 12 damage ends the fight.
        Player player = new Player { Name = "TestHero", HealthPoints = 1 };

        FinalFightOutcome outcome = Play("a\n", player, CreateDragon(20), GetMessages(language), [1, 20, 12], out string output);

        Assert.AreEqual(FinalFightOutcome.Lost, outcome);
        StringAssert.Contains(output, "\n" + expected + "\nSmolderfang ");
    }
}
