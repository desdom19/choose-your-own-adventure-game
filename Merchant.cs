public enum PurchaseResult
{
    Bought,
    NotEnoughGold,
    AlreadyOwned,
}

// The Merchant on the south path. Keeps no state between visits: what's for sale
// is worked out from the Player's equipment each time.
public static class Merchant
{
    public const int ArmorPrice = 70;
    public const int MagicalWeaponPrice = 90;

    public static void Visit(Player player, Messages messages)
    {
        Console.WriteLine(messages.GetMessage("merchant_greeting"));

        while (true)
        {
            if (NothingLeftToSell(player))
            {
                Console.WriteLine(messages.GetMessage("merchant_nothing_left"));
                return;
            }

            ShowMenu(player, messages);
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim();

            // No more input (for example, the console closed): leave the Merchant.
            if (input == null || input == "3")
                return;

            if (input == "1")
                ShowPurchaseResult(BuyArmor(player), messages, () => string.Format(
                    messages.GetMessage("merchant_bought_armor"),
                    messages.TranslateArmorForDisplay(player.Armor!.Name),
                    ArmorPrice));
            else if (input == "2")
                ShowPurchaseResult(BuyMagicalWeapon(player), messages, () => string.Format(
                    messages.GetMessage("merchant_bought_magical_weapon"),
                    messages.TranslateWeaponForDisplay(player.Weapon!.Type),
                    MagicalWeaponPrice));
            else
                Console.WriteLine(messages.GetMessage("invalid"));
        }
    }

    private static void ShowMenu(Player player, Messages messages)
    {
        Console.WriteLine(string.Format(messages.GetMessage("merchant_gold_shown"), player.Gold));
        Console.WriteLine(messages.GetMessage("merchant_menu_header"));

        if (player.Armor == null)
        {
            Console.WriteLine(string.Format(
                messages.GetMessage("merchant_menu_armor"),
                messages.TranslateArmorForDisplay(EquipmentFor(player).ArmorName),
                ArmorPrice));
        }

        OccupationEquipment equipment = EquipmentFor(player);
        if (!OwnsMagicalWeapon(player, equipment))
        {
            Console.WriteLine(string.Format(
                messages.GetMessage("merchant_menu_magical_weapon"),
                messages.TranslateWeaponForDisplay(equipment.MagicalWeaponName),
                MagicalWeaponPrice));
        }

        Console.WriteLine(messages.GetMessage("merchant_menu_leave"));
    }

    // The confirmation is built only after a purchase, once the item it names is owned.
    private static void ShowPurchaseResult(PurchaseResult result, Messages messages, Func<string> confirmation)
    {
        if (result == PurchaseResult.Bought)
        {
            Console.WriteLine(confirmation());
        }
        else if (result == PurchaseResult.NotEnoughGold)
        {
            Console.WriteLine(messages.GetMessage("merchant_not_enough_gold"));
        }
        else
        {
            Console.WriteLine(messages.GetMessage("merchant_already_owned"));
        }
    }

    // No console I/O. Gold and equipment change only when the result is Bought.
    private static PurchaseResult BuyArmor(Player player)
    {
        if (player.Armor != null)
            return PurchaseResult.AlreadyOwned;

        if (player.Gold < ArmorPrice)
            return PurchaseResult.NotEnoughGold;

        player.Gold -= ArmorPrice;
        player.Armor = EquipmentFor(player).CreateArmor();
        return PurchaseResult.Bought;
    }

    // No console I/O. Gold and equipment change only when the result is Bought.
    // The new weapon replaces the old one outright; the starting weapon definition is untouched.
    private static PurchaseResult BuyMagicalWeapon(Player player)
    {
        OccupationEquipment equipment = EquipmentFor(player);

        if (OwnsMagicalWeapon(player, equipment))
            return PurchaseResult.AlreadyOwned;

        if (player.Gold < MagicalWeaponPrice)
            return PurchaseResult.NotEnoughGold;

        player.Gold -= MagicalWeaponPrice;
        player.Weapon = equipment.CreateMagicalWeapon();
        return PurchaseResult.Bought;
    }

    private static bool NothingLeftToSell(Player player)
    {
        return player.Armor != null && OwnsMagicalWeapon(player, EquipmentFor(player));
    }

    private static bool OwnsMagicalWeapon(Player player, OccupationEquipment equipment)
    {
        return player.Weapon?.Type == equipment.MagicalWeaponName;
    }

    // Character creation only accepts known occupations, so a missing definition is a bug.
    private static OccupationEquipment EquipmentFor(Player player)
    {
        return OccupationEquipment.For(player.Occupation)
            ?? throw new InvalidOperationException($"No equipment is defined for occupation '{player.Occupation}'.");
    }
}
