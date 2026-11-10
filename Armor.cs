public class Armor
{
    public string Name { get; set; }
    public int Protection { get; set; }

    public Armor(string name, int protection)
    {
        Name = name;
        Protection = protection;
    }
}
