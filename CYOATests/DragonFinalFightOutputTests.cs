using System.IO;
using System.Reflection;

// Characterization tests: the exact text a Dragon Final Fight prints, in every language.
// Every Dragon line must stay the same, character for character, in every language.
[TestClass]
[DoNotParallelize]
public class DragonFinalFightOutputTests
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

    private static Messages GetMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    private static Player CreatePlayer(Die die, int healthPoints)
    {
        return new Player(die)
        {
            Name = "TestHero",
            Race = "Elf",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = healthPoints,
            Weapon = new Weapon("long sword", 12, "-)=====>"),
            Armor = new Armor("enchanted plate armor", 2)
        };
    }

    private static string Capture(string input, Action action)
    {
        Console.SetIn(new StringReader(input));
        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);
        action();
        return writer.ToString().Replace("\r\n", "\n");
    }

    // Four rounds: the Dragon dodges and taunts, then misses; the player misses, then dodges;
    // both deal damage, with the Armor absorbing part of the hit; the player wins.
    private static string PlayWonFight(string language)
    {
        Messages messages = GetMessages(language);

        // Dragon: Strength 10, Agility 10, Health 5. Round 1: taunt 3, attack misses (15).
        // Round 2: attack hits (1), player dodges (1). Round 3: reply 2, attack hits (1),
        // player fails to dodge (20), damage 5.
        Dragon dragon = new Dragon(new ScriptedDie(10, 10, 5, 3, 15, 1, 1, 2, 1, 20, 5));

        // Player: round 1 hits (1), Dragon dodges (5). Round 2 misses (20).
        // Round 3 hits (1), no dodge (20), damage 3. Round 4 hits (1), no dodge (20), damage 4.
        Player player = CreatePlayer(new ScriptedDie(1, 5, 20, 1, 20, 3, 1, 20, 4), 20);

        return Capture("a\na\na\n", () =>
        {
            Combat combat = new Combat(player, dragon, messages);
            bool playerWon = combat.StartCombat();
            new Game().EndGame(dragon, playerWon, combat.PlayerRetreated, messages);
        });
    }

    // The player misses, then the Dragon hits for more than the player's Health Points.
    private static string PlayLostFight(string language)
    {
        Messages messages = GetMessages(language);
        Dragon dragon = new Dragon(new ScriptedDie(10, 10, 5, 1, 20, 12));
        Player player = CreatePlayer(new ScriptedDie(20), 3);

        return Capture("", () =>
        {
            Combat combat = new Combat(player, dragon, messages);
            bool playerWon = combat.StartCombat();
            new Game().EndGame(dragon, playerWon, combat.PlayerRetreated, messages);
        });
    }

    // The encounter on the north path: the player attacks, then retreats after one round.
    private static string PlayEncounterThenRetreatMidFight(string language)
    {
        Messages messages = GetMessages(language);
        Dragon dragon = new Dragon(new ScriptedDie(10, 10, 5, 15));
        Player player = CreatePlayer(new ScriptedDie(20), 20);

        return Capture("x\na\nr\n", () => InvokeEncounter(player, dragon, messages));
    }

    // The encounter on the north path: the player retreats before the fight.
    private static string PlayEncounterThenRetreat(string language)
    {
        Messages messages = GetMessages(language);
        Dragon dragon = new Dragon(new ScriptedDie(7, 8, 9));
        Player player = CreatePlayer(new ScriptedDie(), 20);

        return Capture("r\n", () => InvokeEncounter(player, dragon, messages));
    }

    private static void InvokeEncounter(Player player, Dragon dragon, Messages messages)
    {
        MethodInfo? method = typeof(Game).GetMethod("HandleEncounter", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, "The encounter method was not found.");
        method!.Invoke(new Game(), new object[] { player, dragon, messages });
    }

    private static void AssertSameText(string expected, string actual)
    {
        Assert.AreEqual(expected.Replace("\r\n", "\n").TrimEnd(), actual.TrimEnd());
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void WonFight_PrintsExactText(string language)
    {
        AssertSameText(ExpectedWon[language], PlayWonFight(language));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void LostFight_PrintsExactText(string language)
    {
        AssertSameText(ExpectedLost[language], PlayLostFight(language));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void EncounterThenRetreatMidFight_PrintsExactText(string language)
    {
        AssertSameText(ExpectedMidretreat[language], PlayEncounterThenRetreatMidFight(language));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void EncounterThenRetreat_PrintsExactText(string language)
    {
        AssertSameText(ExpectedRetreat[language], PlayEncounterThenRetreat(language));
    }

    private static readonly Dictionary<string, string> ExpectedWon = new()
    {
        ["English"] = """
        ===== Combat Stats =====
        --- Player ---
        Name: TestHero
        Strength: 15
        Agility: 12
        Health Points: 20
        Protection: 2
        Weapon: long sword
        Weapon's Maximum Damage Points: 12

        --- Dragon ---
        Name: Smolderfang
        Strength: 10
        Agility: 10
        Health Points: 5
        Weapon: claws
        Weapon's Maximum Damage Points: 12
        As a Fighter, you withdraw your long sword and attack the dragon.
        -)=====>
        You hit Smolderfang.
        Smolderfang successfully defends your attack.
        Try harder, Elf! You're making me yawn!
        Smolderfang attacks TestHero with claws!
        <<< claws >>>
        Smolderfang missed TestHero.
        Enter 'a' to attack or 'r' to retreat.
        As a Fighter, you withdraw your long sword and attack the dragon.
        -)=====>
        You missed Smolderfang.
        Smolderfang attacks TestHero with claws!
        <<< claws >>>
        Smolderfang hits TestHero.
        TestHero successfully defends the attack.
        Enter 'a' to attack or 'r' to retreat.
        As a Fighter, you withdraw your long sword and attack the dragon.
        -)=====>
        You hit Smolderfang.
        You deal 3 damage.
        Smolderfang's health points are now 2.
        Impressive! You might actually be worth toasting...
        Smolderfang attacks TestHero with claws!
        <<< claws >>>
        Smolderfang hits TestHero.
        Smolderfang deals 5 damage.
        Armor (enchanted plate armor): 2 damage absorbed, 3 damage taken.
        TestHero's health points are now 17.
        Enter 'a' to attack or 'r' to retreat.
        As a Fighter, you withdraw your long sword and attack the dragon.
        -)=====>
        You hit Smolderfang.
        You deal 4 damage.
        Smolderfang's health points are now -2.
        With the last words of 'Arggg!', you have defeated the dragon.
        With the dragon vanquished, you collect the horde of gold, return home, rich and famous. The End.
        """,
        ["Spanish"] = """
        ===== Estadisticas de Combate =====
        --- Jugador ---
        Nombre: TestHero
        Fuerza: 15
        Agilidad: 12
        Puntos de Vida: 20
        Proteccion: 2
        Arma: espada larga
        Puntos Maximos de Dano del Arma: 12

        --- Dragon ---
        Nombre: Smolderfang
        Fuerza: 10
        Agilidad: 10
        Puntos de Vida: 5
        Arma: garras
        Puntos Maximos de Dano del Arma: 12
        Como Guerrero, sacas tu espada larga y atacas al dragon.
        -)=====>
        Golpeaste a Smolderfang.
        Smolderfang defendio exitosamente tu ataque.
        Esfuerzate mas, Elfo! Me estas dando sueno!
        Smolderfang ataca a TestHero con garras!
        <<< claws >>>
        Smolderfang fallo contra TestHero.
        Escribe 'a' para atacar o 'r' para retirarse.
        Como Guerrero, sacas tu espada larga y atacas al dragon.
        -)=====>
        Fallaste contra Smolderfang.
        Smolderfang ataca a TestHero con garras!
        <<< claws >>>
        Smolderfang golpea a TestHero.
        TestHero defendio exitosamente el ataque.
        Escribe 'a' para atacar o 'r' para retirarse.
        Como Guerrero, sacas tu espada larga y atacas al dragon.
        -)=====>
        Golpeaste a Smolderfang.
        Haces 3 de dano.
        Los puntos de vida de Smolderfang ahora son 2.
        Impresionante! Tal vez realmente valga la pena tostarte...
        Smolderfang ataca a TestHero con garras!
        <<< claws >>>
        Smolderfang golpea a TestHero.
        Smolderfang hace 5 de dano.
        Armadura (armadura de placas encantada): 2 de dano absorbido, 3 de dano recibido.
        Los puntos de vida de TestHero ahora son 17.
        Escribe 'a' para atacar o 'r' para retirarse.
        Como Guerrero, sacas tu espada larga y atacas al dragon.
        -)=====>
        Golpeaste a Smolderfang.
        Haces 4 de dano.
        Los puntos de vida de Smolderfang ahora son -2.
        Con las ultimas palabras de 'Arggg!', has derrotado al dragon.
        Con el dragon vencido, recoges el tesoro de oro y regresas a casa, rico y famoso. Fin.
        """,
        ["French"] = """
        ===== Statistiques de Combat =====
        --- Joueur ---
        Nom: TestHero
        Force: 15
        Agilite: 12
        Points de Vie: 20
        Protection: 2
        Arme: epee longue
        Points de Dommages Maximum: 12

        --- Dragon ---
        Nom: Smolderfang
        Force: 10
        Agilite: 10
        Points de Vie: 5
        Arme: griffes
        Points de Dommages Maximum: 12
        En tant que Guerrier, vous sortez votre epee longue et attaquez le dragon.
        -)=====>
        Vous avez touche Smolderfang.
        Smolderfang defend avec succes votre attaque.
        Essaie plus fort, Elfe! Tu me fais bailler!
        Smolderfang attaque TestHero avec griffes!
        <<< claws >>>
        Smolderfang a manque TestHero.
        Tapez 'a' pour attaquer ou 'r' pour vous retirer.
        En tant que Guerrier, vous sortez votre epee longue et attaquez le dragon.
        -)=====>
        Vous avez manque Smolderfang.
        Smolderfang attaque TestHero avec griffes!
        <<< claws >>>
        Smolderfang touche TestHero.
        TestHero defend avec succes l'attaque.
        Tapez 'a' pour attaquer ou 'r' pour vous retirer.
        En tant que Guerrier, vous sortez votre epee longue et attaquez le dragon.
        -)=====>
        Vous avez touche Smolderfang.
        Vous infligez 3 degats.
        Les points de vie de Smolderfang sont maintenant de 2.
        Impressionnant! Tu vaux peut-etre vraiment la peine d'etre grille...
        Smolderfang attaque TestHero avec griffes!
        <<< claws >>>
        Smolderfang touche TestHero.
        Smolderfang inflige 5 degats.
        Armure (armure de plaques enchantee): 2 degats absorbes, 3 degats subis.
        Les points de vie de TestHero sont maintenant de 17.
        Tapez 'a' pour attaquer ou 'r' pour vous retirer.
        En tant que Guerrier, vous sortez votre epee longue et attaquez le dragon.
        -)=====>
        Vous avez touche Smolderfang.
        Vous infligez 4 degats.
        Les points de vie de Smolderfang sont maintenant de -2.
        Avec les derniers mots 'Arggg!', vous avez vaincu le dragon.
        Le dragon vaincu, vous recuperez le tresor d'or et rentrez chez vous, riche et celebre. Fin.
        """,
        ["Italian"] = """
        ===== Statistiche di Combattimento =====
        --- Giocatore ---
        Nome: TestHero
        Forza: 15
        Agilita: 12
        Punti Vita: 20
        Protezione: 2
        Arma: spada lunga
        Danno Massimo dell'Arma: 12

        --- Drago ---
        Nome: Smolderfang
        Forza: 10
        Agilita: 10
        Punti Vita: 5
        Arma: artigli
        Danno Massimo dell'Arma: 12
        In quanto Guerriero, impugni la tua arma (spada lunga) e attacchi il drago.
        -)=====>
        Hai colpito Smolderfang.
        Smolderfang para il tuo attacco.
        Impegnati di piu, Elfo! Mi fai sbadigliare!
        Smolderfang attacca TestHero con artigli!
        <<< claws >>>
        Smolderfang manca TestHero.
        Scrivi 'a' per attaccare o 'r' per ritirarti.
        In quanto Guerriero, impugni la tua arma (spada lunga) e attacchi il drago.
        -)=====>
        Hai mancato Smolderfang.
        Smolderfang attacca TestHero con artigli!
        <<< claws >>>
        Smolderfang colpisce TestHero.
        TestHero para l'attacco.
        Scrivi 'a' per attaccare o 'r' per ritirarti.
        In quanto Guerriero, impugni la tua arma (spada lunga) e attacchi il drago.
        -)=====>
        Hai colpito Smolderfang.
        Danni inflitti: 3.
        I punti vita di Smolderfang ora sono 2.
        Notevole! Forse vale davvero la pena di arrostirti...
        Smolderfang attacca TestHero con artigli!
        <<< claws >>>
        Smolderfang colpisce TestHero.
        Danni inflitti da Smolderfang: 5.
        Armatura (armatura a piastre incantata): 2 danni assorbiti, 3 danni subiti.
        I punti vita di TestHero ora sono 17.
        Scrivi 'a' per attaccare o 'r' per ritirarti.
        In quanto Guerriero, impugni la tua arma (spada lunga) e attacchi il drago.
        -)=====>
        Hai colpito Smolderfang.
        Danni inflitti: 4.
        I punti vita di Smolderfang ora sono -2.
        Con le ultime parole 'Arggg!', il drago cade sconfitto.
        Con il drago sconfitto, raccogli il tesoro d'oro e torni a casa ricco e famoso. Fine.
        """,
    };

    private static readonly Dictionary<string, string> ExpectedLost = new()
    {
        ["English"] = """
        ===== Combat Stats =====
        --- Player ---
        Name: TestHero
        Strength: 15
        Agility: 12
        Health Points: 3
        Protection: 2
        Weapon: long sword
        Weapon's Maximum Damage Points: 12

        --- Dragon ---
        Name: Smolderfang
        Strength: 10
        Agility: 10
        Health Points: 5
        Weapon: claws
        Weapon's Maximum Damage Points: 12
        As a Fighter, you withdraw your long sword and attack the dragon.
        -)=====>
        You missed Smolderfang.
        Smolderfang attacks TestHero with claws!
        <<< claws >>>
        Smolderfang hits TestHero.
        Smolderfang deals 12 damage.
        Armor (enchanted plate armor): 2 damage absorbed, 10 damage taken.
        TestHero's health points are now 0.
        TestHero has been defeated.
        The dragon laughs as your eyesight fades. You are defeated. Your family mourns their loss. The End.
        """,
        ["Spanish"] = """
        ===== Estadisticas de Combate =====
        --- Jugador ---
        Nombre: TestHero
        Fuerza: 15
        Agilidad: 12
        Puntos de Vida: 3
        Proteccion: 2
        Arma: espada larga
        Puntos Maximos de Dano del Arma: 12

        --- Dragon ---
        Nombre: Smolderfang
        Fuerza: 10
        Agilidad: 10
        Puntos de Vida: 5
        Arma: garras
        Puntos Maximos de Dano del Arma: 12
        Como Guerrero, sacas tu espada larga y atacas al dragon.
        -)=====>
        Fallaste contra Smolderfang.
        Smolderfang ataca a TestHero con garras!
        <<< claws >>>
        Smolderfang golpea a TestHero.
        Smolderfang hace 12 de dano.
        Armadura (armadura de placas encantada): 2 de dano absorbido, 10 de dano recibido.
        Los puntos de vida de TestHero ahora son 0.
        TestHero ha sido derrotado.
        El dragon se rie mientras tu vista se desvanece. Has sido derrotado. Tu familia llora tu perdida. Fin.
        """,
        ["French"] = """
        ===== Statistiques de Combat =====
        --- Joueur ---
        Nom: TestHero
        Force: 15
        Agilite: 12
        Points de Vie: 3
        Protection: 2
        Arme: epee longue
        Points de Dommages Maximum: 12

        --- Dragon ---
        Nom: Smolderfang
        Force: 10
        Agilite: 10
        Points de Vie: 5
        Arme: griffes
        Points de Dommages Maximum: 12
        En tant que Guerrier, vous sortez votre epee longue et attaquez le dragon.
        -)=====>
        Vous avez manque Smolderfang.
        Smolderfang attaque TestHero avec griffes!
        <<< claws >>>
        Smolderfang touche TestHero.
        Smolderfang inflige 12 degats.
        Armure (armure de plaques enchantee): 2 degats absorbes, 10 degats subis.
        Les points de vie de TestHero sont maintenant de 0.
        TestHero a ete vaincu.
        Le dragon rit tandis que votre vue s'eteint. Vous etes vaincu. Votre famille pleure votre perte. Fin.
        """,
        ["Italian"] = """
        ===== Statistiche di Combattimento =====
        --- Giocatore ---
        Nome: TestHero
        Forza: 15
        Agilita: 12
        Punti Vita: 3
        Protezione: 2
        Arma: spada lunga
        Danno Massimo dell'Arma: 12

        --- Drago ---
        Nome: Smolderfang
        Forza: 10
        Agilita: 10
        Punti Vita: 5
        Arma: artigli
        Danno Massimo dell'Arma: 12
        In quanto Guerriero, impugni la tua arma (spada lunga) e attacchi il drago.
        -)=====>
        Hai mancato Smolderfang.
        Smolderfang attacca TestHero con artigli!
        <<< claws >>>
        Smolderfang colpisce TestHero.
        Danni inflitti da Smolderfang: 12.
        Armatura (armatura a piastre incantata): 2 danni assorbiti, 10 danni subiti.
        I punti vita di TestHero ora sono 0.
        TestHero viene sconfitto.
        Il drago ride mentre la tua vista si spegne. Sei sconfitto. La tua famiglia piange la tua perdita. Fine.
        """,
    };

    private static readonly Dictionary<string, string> ExpectedMidretreat = new()
    {
        ["English"] = """
        You take the North path. After walking for hours, the air grows colder, and the trees become more sparse. You are confronted with a dragon. The dragon has a hoard of gold behind it.
        The dragon says 'I am Smolderfang. Leave or perish!'
        Your magic ring gives you the stats on the dragon.
        ----- Dragon Stats -----
        Name: Smolderfang
        Strength: 10
        Agility: 10
        Health Points: 5
        Weapon: claws
        Weapon's Maximum Damage Points: 12
        Do you wish to retreat or attack? Enter 'r' for retreat. Enter 'a' for attack.
        Enter choice: Invalid choice. Please try again.
        Do you wish to retreat or attack? Enter 'r' for retreat. Enter 'a' for attack.
        Enter choice: ===== Combat Stats =====
        --- Player ---
        Name: TestHero
        Strength: 15
        Agility: 12
        Health Points: 20
        Protection: 2
        Weapon: long sword
        Weapon's Maximum Damage Points: 12

        --- Dragon ---
        Name: Smolderfang
        Strength: 10
        Agility: 10
        Health Points: 5
        Weapon: claws
        Weapon's Maximum Damage Points: 12
        As a Fighter, you withdraw your long sword and attack the dragon.
        -)=====>
        You missed Smolderfang.
        Smolderfang attacks TestHero with claws!
        <<< claws >>>
        Smolderfang missed TestHero.
        Enter 'a' to attack or 'r' to retreat.
        You hastily retreat licking your wounds.
        """,
        ["Spanish"] = """
        Tomas el camino del norte. Despues de caminar durante horas, el aire se vuelve mas frio y los arboles mas escasos. Te enfrentas a un dragon. El dragon tiene una horda de oro detras de el.
        El dragon dice 'Soy Smolderfang. Vete o perece!'
        Tu anillo magico te da las estadisticas del dragon.
        ----- Estadisticas del Dragon -----
        Nombre: Smolderfang
        Fuerza: 10
        Agilidad: 10
        Puntos de Vida: 5
        Arma: garras
        Puntos Maximos de Dano del Arma: 12
        Deseas retirarte o atacar? Escribe 'r' para retirarte. Escribe 'a' para atacar.
        Escribe tu opcion: Opcion invalida. Intenta de nuevo.
        Deseas retirarte o atacar? Escribe 'r' para retirarte. Escribe 'a' para atacar.
        Escribe tu opcion: ===== Estadisticas de Combate =====
        --- Jugador ---
        Nombre: TestHero
        Fuerza: 15
        Agilidad: 12
        Puntos de Vida: 20
        Proteccion: 2
        Arma: espada larga
        Puntos Maximos de Dano del Arma: 12

        --- Dragon ---
        Nombre: Smolderfang
        Fuerza: 10
        Agilidad: 10
        Puntos de Vida: 5
        Arma: garras
        Puntos Maximos de Dano del Arma: 12
        Como Guerrero, sacas tu espada larga y atacas al dragon.
        -)=====>
        Fallaste contra Smolderfang.
        Smolderfang ataca a TestHero con garras!
        <<< claws >>>
        Smolderfang fallo contra TestHero.
        Escribe 'a' para atacar o 'r' para retirarse.
        Te retiras apresuradamente mientras lames tus heridas.
        """,
        ["French"] = """
        Vous prenez le chemin du nord. Apres avoir marche pendant des heures, l'air devient plus froid et les arbres plus rares. Vous etes confronte a un dragon. Le dragon a un tresor d'or derriere lui.
        Le dragon dit 'Je suis Smolderfang. Pars ou peris !'
        Votre anneau magique vous donne les statistiques du dragon.
        ----- Statistiques du Dragon -----
        Nom: Smolderfang
        Force: 10
        Agilite: 10
        Points de Vie: 5
        Arme: griffes
        Points de Dommages Maximum: 12
        Souhaitez-vous vous retirer ou attaquer ? Tapez 'r' pour vous retirer. Tapez 'a' pour attaquer.
        Entrez votre choix : Choix invalide. Veuillez reessayer.
        Souhaitez-vous vous retirer ou attaquer ? Tapez 'r' pour vous retirer. Tapez 'a' pour attaquer.
        Entrez votre choix : ===== Statistiques de Combat =====
        --- Joueur ---
        Nom: TestHero
        Force: 15
        Agilite: 12
        Points de Vie: 20
        Protection: 2
        Arme: epee longue
        Points de Dommages Maximum: 12

        --- Dragon ---
        Nom: Smolderfang
        Force: 10
        Agilite: 10
        Points de Vie: 5
        Arme: griffes
        Points de Dommages Maximum: 12
        En tant que Guerrier, vous sortez votre epee longue et attaquez le dragon.
        -)=====>
        Vous avez manque Smolderfang.
        Smolderfang attaque TestHero avec griffes!
        <<< claws >>>
        Smolderfang a manque TestHero.
        Tapez 'a' pour attaquer ou 'r' pour vous retirer.
        Vous vous retirez rapidement en pansant vos blessures.
        """,
        ["Italian"] = """
        Prendi il sentiero a nord. Dopo ore di cammino, l'aria si fa fredda e gli alberi si diradano. Ti trovi davanti a un drago. Dietro di lui brilla un tesoro d'oro.
        Il drago dice 'Io sono Smolderfang. Vattene o morirai!'
        Il tuo anello magico ti rivela le statistiche del drago.
        ----- Statistiche del Drago -----
        Nome: Smolderfang
        Forza: 10
        Agilita: 10
        Punti Vita: 5
        Arma: artigli
        Danno Massimo dell'Arma: 12
        Vuoi ritirarti o attaccare? Scrivi 'r' per ritirarti. Scrivi 'a' per attaccare.
        Inserisci la tua scelta: Scelta non valida. Riprova.
        Vuoi ritirarti o attaccare? Scrivi 'r' per ritirarti. Scrivi 'a' per attaccare.
        Inserisci la tua scelta: ===== Statistiche di Combattimento =====
        --- Giocatore ---
        Nome: TestHero
        Forza: 15
        Agilita: 12
        Punti Vita: 20
        Protezione: 2
        Arma: spada lunga
        Danno Massimo dell'Arma: 12

        --- Drago ---
        Nome: Smolderfang
        Forza: 10
        Agilita: 10
        Punti Vita: 5
        Arma: artigli
        Danno Massimo dell'Arma: 12
        In quanto Guerriero, impugni la tua arma (spada lunga) e attacchi il drago.
        -)=====>
        Hai mancato Smolderfang.
        Smolderfang attacca TestHero con artigli!
        <<< claws >>>
        Smolderfang manca TestHero.
        Scrivi 'a' per attaccare o 'r' per ritirarti.
        Ti ritiri in fretta leccandoti le ferite.
        """,
    };

    private static readonly Dictionary<string, string> ExpectedRetreat = new()
    {
        ["English"] = """
        You take the North path. After walking for hours, the air grows colder, and the trees become more sparse. You are confronted with a dragon. The dragon has a hoard of gold behind it.
        The dragon says 'I am Smolderfang. Leave or perish!'
        Your magic ring gives you the stats on the dragon.
        ----- Dragon Stats -----
        Name: Smolderfang
        Strength: 7
        Agility: 8
        Health Points: 9
        Weapon: claws
        Weapon's Maximum Damage Points: 12
        Do you wish to retreat or attack? Enter 'r' for retreat. Enter 'a' for attack.
        Enter choice: You quickly retreat. You come home poor but alive to adventure another day. The End.
        """,
        ["Spanish"] = """
        Tomas el camino del norte. Despues de caminar durante horas, el aire se vuelve mas frio y los arboles mas escasos. Te enfrentas a un dragon. El dragon tiene una horda de oro detras de el.
        El dragon dice 'Soy Smolderfang. Vete o perece!'
        Tu anillo magico te da las estadisticas del dragon.
        ----- Estadisticas del Dragon -----
        Nombre: Smolderfang
        Fuerza: 7
        Agilidad: 8
        Puntos de Vida: 9
        Arma: garras
        Puntos Maximos de Dano del Arma: 12
        Deseas retirarte o atacar? Escribe 'r' para retirarte. Escribe 'a' para atacar.
        Escribe tu opcion: Te has retirado de la batalla.
        """,
        ["French"] = """
        Vous prenez le chemin du nord. Apres avoir marche pendant des heures, l'air devient plus froid et les arbres plus rares. Vous etes confronte a un dragon. Le dragon a un tresor d'or derriere lui.
        Le dragon dit 'Je suis Smolderfang. Pars ou peris !'
        Votre anneau magique vous donne les statistiques du dragon.
        ----- Statistiques du Dragon -----
        Nom: Smolderfang
        Force: 7
        Agilite: 8
        Points de Vie: 9
        Arme: griffes
        Points de Dommages Maximum: 12
        Souhaitez-vous vous retirer ou attaquer ? Tapez 'r' pour vous retirer. Tapez 'a' pour attaquer.
        Entrez votre choix : Vous vous etes retire du combat.
        """,
        ["Italian"] = """
        Prendi il sentiero a nord. Dopo ore di cammino, l'aria si fa fredda e gli alberi si diradano. Ti trovi davanti a un drago. Dietro di lui brilla un tesoro d'oro.
        Il drago dice 'Io sono Smolderfang. Vattene o morirai!'
        Il tuo anello magico ti rivela le statistiche del drago.
        ----- Statistiche del Drago -----
        Nome: Smolderfang
        Forza: 7
        Agilita: 8
        Punti Vita: 9
        Arma: artigli
        Danno Massimo dell'Arma: 12
        Vuoi ritirarti o attaccare? Scrivi 'r' per ritirarti. Scrivi 'a' per attaccare.
        Inserisci la tua scelta: Ti ritiri in fretta. Torni a casa povero ma vivo, pronto per una nuova avventura. Fine.
        """,
    };
}
