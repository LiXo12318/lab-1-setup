namespace MenuApi.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}
