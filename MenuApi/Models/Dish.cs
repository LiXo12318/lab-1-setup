namespace MenuApi.Models;

public class Dish
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public bool IsAvailable { get; set; }
    public int Calories { get; set; }
    public List<string> Allergens { get; set; } = new();

    public ICollection<DailySpecial> DailySpecials { get; set; } = new List<DailySpecial>();
}
