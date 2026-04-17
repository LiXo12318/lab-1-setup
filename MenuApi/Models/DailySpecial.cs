namespace MenuApi.Models;

public class DailySpecial
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public Dish Dish { get; set; } = null!;
    public DateOnly Date { get; set; }
    public int DiscountPercent { get; set; }
    public decimal SpecialPrice { get; set; }
}
