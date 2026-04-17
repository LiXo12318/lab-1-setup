namespace MenuApi.Contracts;

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class CreateDishRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int Calories { get; set; }
    public List<string> Allergens { get; set; } = new();
}

public class UpdateDishRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public bool IsAvailable { get; set; }
    public int Calories { get; set; }
    public List<string> Allergens { get; set; } = new();
}

public class CreateDailySpecialRequest
{
    public int DishId { get; set; }
    public DateOnly Date { get; set; }
    public int DiscountPercent { get; set; }
}

public class MenuCategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public List<MenuDishResponse> Dishes { get; set; } = new();
}

public class MenuDishResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public bool IsAvailable { get; set; }
    public int Calories { get; set; }
    public List<string> Allergens { get; set; } = new();
}

public class DailySpecialResponse
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public DateOnly Date { get; set; }
    public int DiscountPercent { get; set; }
    public decimal SpecialPrice { get; set; }
}
