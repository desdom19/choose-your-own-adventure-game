// The one place where each occupation's equipment is defined.
// Later the Merchant's Magical Weapon for each occupation lives here too.
public class OccupationEquipment
{
    // Every Armor gives the same Protection, so every player gets the same value for the price.
    private const int ArmorProtection = 3;

    private static readonly Dictionary<string, OccupationEquipment> _byOccupation =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Fighter"] = new OccupationEquipment(
                new Weapon("long sword", 12, "-)=====>"),
                new Armor("enchanted plate armor", ArmorProtection)),
            ["Magician"] = new OccupationEquipment(
                new Weapon("lightning bolt spell", 12, "zap~~~~~~"),
                new Armor("warded robe", ArmorProtection)),
            ["Thief"] = new OccupationEquipment(
                new Weapon("dagger", 6, "-)==>"),
                new Armor("shadow leathers", ArmorProtection)),
            ["Archer"] = new OccupationEquipment(
                new Weapon("long bow", 8, "}    -->"),
                new Armor("elven cloak", ArmorProtection)),
        };

    // Templates only: never handed out, because Weapon and Armor are mutable.
    private readonly Weapon _startingWeapon;
    private readonly Armor _armor;

    private OccupationEquipment(Weapon startingWeapon, Armor armor)
    {
        _startingWeapon = startingWeapon;
        _armor = armor;
    }

    // Returns null for an unknown occupation.
    public static OccupationEquipment? For(string occupation)
    {
        return _byOccupation.TryGetValue(occupation, out OccupationEquipment? equipment) ? equipment : null;
    }

    // A new Weapon each time, so one player's weapon is never shared with another's.
    public Weapon CreateStartingWeapon()
    {
        return new Weapon(_startingWeapon.Type, _startingWeapon.MaxDamage, _startingWeapon.AsciiArt);
    }

    public string ArmorName => _armor.Name;

    // A new Armor each time, so one player's Armor is never shared with another's.
    public Armor CreateArmor()
    {
        return new Armor(_armor.Name, _armor.Protection);
    }
}
