// The one place where each occupation's equipment is defined.
// Later the Merchant's Magical Weapon and Armor for each occupation live here too.
public class OccupationEquipment
{
    private static readonly Dictionary<string, OccupationEquipment> _byOccupation =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Fighter"] = new OccupationEquipment(new Weapon("long sword", 12, "-)=====>")),
            ["Magician"] = new OccupationEquipment(new Weapon("lightning bolt spell", 12, "zap~~~~~~")),
            ["Thief"] = new OccupationEquipment(new Weapon("dagger", 6, "-)==>")),
            ["Archer"] = new OccupationEquipment(new Weapon("long bow", 8, "}    -->")),
        };

    // A template only: never handed out, because Weapon is mutable.
    private readonly Weapon _startingWeapon;

    private OccupationEquipment(Weapon startingWeapon)
    {
        _startingWeapon = startingWeapon;
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
}
