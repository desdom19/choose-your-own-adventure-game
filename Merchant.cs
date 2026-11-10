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

    public static void Visit(Player player, Messages messages)
    {
        Console.WriteLine(messages.GetMessage("merchant_greeting"));

        while (true)
        {
            ShowMenu(player, messages);
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim();

            // No more input (for example, the console closed): leave the Merchant.
            if (input == null || input == "3")
                return;

            if (input == "1")
                ShowPurchaseResult(BuyArmor(player), player, messages);
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

        Console.WriteLine(messages.GetMessage("merchant_menu_leave"));
    }

    private static void ShowPurchaseResult(PurchaseResult result, Player player, Messages messages)
    {
        if (result == PurchaseResult.Bought)
        {
            Console.WriteLine(string.Format(
                messages.GetMessage("merchant_bought_armor"),
                messages.TranslateArmorForDisplay(player.Armor!.Name),
                ArmorPrice));
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

    // Character creation only accepts known occupations, so a missing definition is a bug.
    private static OccupationEquipment EquipmentFor(Player player)
    {
        return OccupationEquipment.For(player.Occupation)
            ?? throw new InvalidOperationException($"No equipment is defined for occupation '{player.Occupation}'.");
    }
}
