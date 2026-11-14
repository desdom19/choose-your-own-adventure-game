using static ScriptedFinalFight;

// A Monster's Health Points stop at 0, just like the player's.
[TestClass]
[DoNotParallelize]
public class MonsterHealthPointsTests
{
    // Strength 10, Agility 1 (the Goblin's is 5 + 1), Health Points 2.
    private static Monster CreateMonster(string monsterName) => monsterName switch
    {
        "Dragon" => new Dragon(new ScriptedDie(10, 1, 2)),
        "Goblin" => new Goblin(new ScriptedDie(10, 1, 2)),
        _ => throw new ArgumentException(monsterName),
    };

    [TestMethod]
    [DataRow("Dragon", "English", "Smolderfang's health points are now 0.")]
    [DataRow("Dragon", "Spanish", "Los puntos de vida de Smolderfang ahora son 0.")]
    [DataRow("Dragon", "French", "Les points de vie de Smolderfang sont maintenant de 0.")]
    [DataRow("Dragon", "Italian", "I punti vita di Smolderfang ora sono 0.")]
    [DataRow("Goblin", "English", "Gritknuckle's health points are now 0.")]
    [DataRow("Goblin", "Spanish", "Los puntos de vida de Gritknuckle ahora son 0.")]
    [DataRow("Goblin", "French", "Les points de vie de Gritknuckle sont maintenant de 0.")]
    [DataRow("Goblin", "Italian", "I punti vita di Gritknuckle ora sono 0.")]
    public void PlayerStrike_DealingMoreThanTheMonsterHasLeft_StopsItsHealthPointsAtZero(string monsterName, string language, string expectedHealthLine)
    {
        Monster monster = CreateMonster(monsterName);
        Player player = new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 10,
            Agility = 10,
            HealthPoints = 30,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };

        // The player hits (1), the defense roll of 20 never dodges, and 5 damage beats 2 Health Points.
        FinalFightOutcome outcome = Play("a\n", player, monster, GetMessages(language), [1, 20, 5], out string output);

        Assert.AreEqual(FinalFightOutcome.Won, outcome);
        Assert.AreEqual(0, monster.HealthPoints);
        StringAssert.Contains(output, expectedHealthLine);
    }
}
