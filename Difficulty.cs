// The player's choice of how strong the Monsters are. It changes every Monster stat by
// the same amount after the stats are rolled, then keeps each stat within its limits so
// the Monster can still fight and be fought. It never rolls dice or changes a roll.
public sealed class Difficulty
{
    private const int EasyStatChange = -3;
    private const int NormalStatChange = 0;
    private const int HardStatChange = 3;

    // Strength and Agility stay on the same 1-20 scale as the d20 they're tested against.
    private const int LowestStrength = 1;
    private const int HighestStrength = 20;
    private const int LowestAgility = 1;
    private const int HighestAgility = 20;

    // A Monster starting at 0 Health Points would count as defeated after the player's
    // first attack, even a miss. Health Points have no upper limit.
    private const int LowestHealthPoints = 1;
    private const int LowestWeaponMaxDamage = 1;

    public static readonly Difficulty Easy = new Difficulty(EasyStatChange);
    public static readonly Difficulty Normal = new Difficulty(NormalStatChange);
    public static readonly Difficulty Hard = new Difficulty(HardStatChange);

    private readonly int _statChange;

    private Difficulty(int statChange)
    {
        _statChange = statChange;
    }

    public int ChangeStrength(int rolled) => Math.Clamp(rolled + _statChange, LowestStrength, HighestStrength);
    public int ChangeAgility(int rolled) => Math.Clamp(rolled + _statChange, LowestAgility, HighestAgility);
    public int ChangeHealthPoints(int rolled) => Math.Max(rolled + _statChange, LowestHealthPoints);
    public int ChangeWeaponMaxDamage(int maxDamage) => Math.Max(maxDamage + _statChange, LowestWeaponMaxDamage);
}
