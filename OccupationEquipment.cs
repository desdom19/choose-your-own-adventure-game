// The one place where each occupation's equipment is defined: its starting weapon,
// the Magical Weapon the Merchant sells, and its Armor.
public class OccupationEquipment
{
    // Every Armor gives the same Protection, so every player gets the same value for the price.
    private const int ArmorProtection = 3;

    // Every Magical Weapon adds the same max damage to its starting weapon, so the strength
    // order of occupations stays the same and the price is fair to everyone.
    private const int MagicalWeaponDamageBonus = 4;

    private static readonly Dictionary<string, OccupationEquipment> _byOccupation =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Fighter"] = new OccupationEquipment(
                new Weapon("long sword", 12, "-)=====>"),
                magicalWeaponName: "flaming sword", magicalWeaponArt: "-)=====>~*~*",
                new Armor("enchanted plate armor", ArmorProtection)),
            ["Magician"] = new OccupationEquipment(
                new Weapon("lightning bolt spell", 12, "zap~~~~~~"),
                magicalWeaponName: "chain lightning spell", magicalWeaponArt: "zap~~zap~~zap~~",
                new Armor("warded robe", ArmorProtection)),
            ["Thief"] = new OccupationEquipment(
                new Weapon("dagger", 6, "-)==>"),
                magicalWeaponName: "venom dagger", magicalWeaponArt: "-)==>.:.",
                new Armor("shadow leathers", ArmorProtection)),
            ["Archer"] = new OccupationEquipment(
                new Weapon("long bow", 8, "}    -->"),
                magicalWeaponName: "elven bow", magicalWeaponArt: "}*   *-->*",
                new Armor("elven cloak", ArmorProtection)),
        };

    // Templates only: never handed out, because Weapon and Armor are mutable.
    private readonly Weapon _startingWeapon;
    private readonly Weapon _magicalWeapon;
    private readonly Armor _armor;

    private OccupationEquipment(Weapon startingWeapon, string magicalWeaponName, string magicalWeaponArt, Armor armor)
    {
        _startingWeapon = startingWeapon;
        _magicalWeapon = new Weapon(
            magicalWeaponName,
            startingWeapon.MaxDamage + MagicalWeaponDamageBonus,
            magicalWeaponArt);
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
        return Copy(_startingWeapon);
    }

    public string MagicalWeaponName => _magicalWeapon.Type;

    // A new Weapon each time, so one player's weapon is never shared with another's.
    public Weapon CreateMagicalWeapon()
    {
        return Copy(_magicalWeapon);
    }

    public string ArmorName => _armor.Name;

    // A new Armor each time, so one player's Armor is never shared with another's.
    public Armor CreateArmor()
    {
        return new Armor(_armor.Name, _armor.Protection);
    }

    private static Weapon Copy(Weapon weapon)
    {
        return new Weapon(weapon.Type, weapon.MaxDamage, weapon.AsciiArt);
    }
}
