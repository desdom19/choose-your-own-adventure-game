[TestClass]
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

    [TestMethod]
    public void Attack_WithForcedRolls_DealsExactDamageToDragon()
    {
        // Attack roll 5 hits (Strength 10), defense roll 15 beats the dragon's Agility 10,
        // damage roll 7 on the long sword's d12.
        Player player = new Player(new ScriptedDie(5, 15, 7))
        {
            Name = "TestHero",
            Occupation = "Fighter",
            Strength = 10,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };

        Dragon dragon = new Dragon(new Die()) { Agility = 10, HealthPoints = 20 };

        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        player.Attack(dragon, messages);

        Assert.AreEqual(13, dragon.HealthPoints);
    }
}
