namespace MenuApi.Services;

public static class MenuBusinessRules
{
    public static void EnsurePositivePrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price must be greater than 0.");
    }

    public static int EnsureDiscountPercent(int discountPercent)
    {
        if (discountPercent is < 1 or > 50)
            throw new ArgumentOutOfRangeException(nameof(discountPercent), "DiscountPercent must be between 1 and 50.");
        return discountPercent;
    }

    public static decimal ComputeSpecialPrice(decimal dishPrice, int discountPercent)
    {
        EnsureDiscountPercent(discountPercent);
        return dishPrice * (1 - discountPercent / 100m);
    }
}
