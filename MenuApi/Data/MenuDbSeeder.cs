using Bogus;
using MenuApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MenuApi.Data;

public static class MenuDbSeeder
{
    private const int CategoryCount = 50;
    private const int DishesPerCategory = 200;
    private const int TargetDailySpecials = 5000;

    private static readonly string[] RestaurantCategoryNames =
    [
        "Appetizers", "Salads", "Soups", "Main Courses", "Desserts",
        "Beverages", "Pizza", "Sushi", "Breakfast", "Vegan"
    ];

    private static readonly string[] AppetizerNames =
    [
        "Bruschetta al Pomodoro", "Garlic Bread", "Fried Calamari", "Spring Rolls",
        "Chicken Wings", "Mozzarella Sticks", "Stuffed Mushrooms", "Nachos Supreme",
        "Onion Rings", "Hummus Platter", "Caprese Skewers", "Shrimp Cocktail",
        "Baked Brie", "Potato Skins", "Mini Sliders", "Crab Cakes",
        "Artichoke Dip", "Edamame", "Samosa Platter", "Charcuterie Board",
        "Oysters Rockefeller", "Clams Casino", "Beef Carpaccio", "Tuna Tartare",
        "Deviled Eggs", "Falafel Bites", "Pigs in Blanket", "Cheese Fondue",
        "Spinach Dip", "Mussels Marinara", "Prosciutto-Wrapped Melon", "Antipasto Plate",
        "Tempura Vegetables", "Chicken Satay", "Lamb Kofta", "Coconut Shrimp",
        "Baked Clams", "Escargot", "Poutine", "Dumplings Trio", "Crostini Assortment"
    ];

    private static readonly string[] SaladNames =
    [
        "Caesar Salad", "Greek Salad", "Garden Salad", "Caprese Salad",
        "Cobb Salad", "Waldorf Salad", "Nicoise Salad", "Asian Chicken Salad",
        "Kale and Quinoa Salad", "Spinach Salad", "Arugula Beet Salad", "Chef Salad",
        "Southwest Salad", "Mediterranean Salad", "Pear Gorgonzola Salad", "Salmon Niçoise",
        "Strawberry Fields Salad", "Chopped Italian Salad", "Buffalo Chicken Salad",
        "Steak Salad", "Shrimp Avocado Salad", "Watermelon Feta Salad", "Cucumber Dill Salad",
        "Roasted Veggie Salad", "Taco Salad", "Seaweed Salad", "Panzanella",
        "Coleslaw Bowl", "Fattoush", "Tabbouleh", "House Greens", "Endive Salad",
        "Radicchio Salad", "Burrata Salad", "Grilled Peach Salad", "Quinoa Power Bowl",
        "Chicken Caesar", "Turkey Club Salad", "BLT Salad", "Salmon Caesar",
        "Crispy Chicken Salad", "Veggie Delight Salad", "Protein Garden Bowl"
    ];

    private static readonly string[] SoupNames =
    [
        "Chicken Noodle Soup", "Tomato Basil Soup", "Minestrone", "French Onion Soup",
        "Clam Chowder", "Lobster Bisque", "Borscht", "Pho Bo", "Miso Soup",
        "Hot and Sour Soup", "Wonton Soup", "Gazpacho", "Lentil Soup", "Split Pea Soup",
        "Corn Chowder", "Beef Barley Soup", "Thai Coconut Soup", "Gumbo",
        "Matzo Ball Soup", "Avgolemono", "Ramen Bowl", "Udon Soup",
        "Seafood Chowder", "Pumpkin Soup", "Broccoli Cheddar Soup", "Chili Bowl",
        "Posole", "Tortilla Soup", "Stracciatella", "Ribollita", "Solyanka",
        "Harira", "Tom Yum", "Laksa", "Caldo Verde", "Scotch Broth",
        "Cabbage Roll Soup", "Italian Wedding Soup", "Chicken Tortilla Soup",
        "Cream of Mushroom", "Cream of Asparagus", "Vichyssoise", "Bouillabaisse",
        "Fish Chowder", "Egg Drop Soup", "Consommé", "Beef Pho", "Vegetable Broth Bowl"
    ];

    private static readonly string[] MainCourseNames =
    [
        "Grilled Salmon", "Ribeye Steak", "Chicken Parmesan", "Beef Wellington",
        "Pan-Seared Duck", "Lamb Chops", "Pork Tenderloin", "Fish and Chips",
        "Chicken Marsala", "Beef Stroganoff", "Shrimp Scampi", "Veal Piccata",
        "Roast Chicken", "BBQ Ribs", "Surf and Turf", "Lobster Tail",
        "Coq au Vin", "Osso Buco", "Chicken Tikka Masala", "Pad Thai",
        "Beef Bourguignon", "Shepherd's Pie", "Fish Tacos", "Chicken Kiev",
        "Stuffed Peppers", "Eggplant Parmesan", "Pasta Carbonara", "Lasagna Bolognese",
        "Chicken Cordon Bleu", "Teriyaki Salmon", "Fajitas Platter", "Jambalaya",
        "Chicken Piccata", "Prime Rib", "Duck Confit", "Seafood Paella",
        "Moussaka", "Chicken Fajitas", "Beef Tacos", "Pork Schnitzel",
        "Salmon Teriyaki", "Chicken Teriyaki", "Grilled Swordfish", "Cioppino",
        "Chicken Pot Pie", "Meatloaf Dinner", "Spaghetti and Meatballs", "Chicken Alfredo",
        "Beef Tenderloin", "Pork Chops", "Chicken Stir-Fry", "Beef Curry"
    ];

    private static readonly string[] DessertNames =
    [
        "Chocolate Lava Cake", "Tiramisu", "New York Cheesecake", "Crème Brûlée",
        "Apple Pie", "Ice Cream Sundae", "Brownie Sundae", "Panna Cotta",
        "Banana Split", "Lemon Tart", "Key Lime Pie", "Carrot Cake",
        "Red Velvet Cake", "Chocolate Mousse", "Bread Pudding", "Profiteroles",
        "Cannoli", "Gelato Trio", "Strawberry Shortcake", "Pecan Pie",
        "Chocolate Soufflé", "Rice Pudding", "Flan", "Baklava",
        "Cheesecake Bites", "Molten Chocolate Cake", "Berry Parfait", "Churros",
        "Affogato", "Sorbet Selection", "Fruit Tart", "Meringue Pie",
        "Black Forest Cake", "Opera Cake", "Eclair", "Macaron Plate",
        "Panna Cotta Berry", "Chocolate Fondue", "Sticky Toffee Pudding", "Trifle",
        "Cobbler", "Cinnamon Roll", "Donut Holes", "Milkshake Deluxe",
        "Waffle Sundae", "Pavlova", "Semifreddo", "Zeppole", "Crepes Suzette"
    ];

    private static readonly string[] BeverageNames =
    [
        "Latte", "Cappuccino", "Espresso", "Americano", "Mocha",
        "Green Tea", "Earl Grey Tea", "Chamomile Tea", "Iced Tea", "Matcha Latte",
        "Orange Juice", "Apple Juice", "Cranberry Juice", "Fresh Lemonade",
        "Sparkling Water", "Cola", "Root Beer", "Ginger Ale", "Tonic Water",
        "Red Wine Glass", "White Wine Glass", "House Red", "House White",
        "Craft Beer", "Lager", "IPA", "Stout", "Cider",
        "Mimosa", "Bellini", "Aperol Spritz", "Mojito", "Margarita",
        "Whiskey Sour", "Old Fashioned", "Martini", "Negroni", "Hot Chocolate",
        "Smoothie Berry", "Smoothie Tropical", "Protein Shake", "Milkshake Vanilla",
        "Milkshake Chocolate", "Frappé", "Cold Brew", "Iced Coffee", "Hot Apple Cider",
        "Kombucha", "Coconut Water", "Mineral Water", "San Pellegrino", "Shirley Temple"
    ];

    private static readonly string[] PizzaNames =
    [
        "Margherita Pizza", "Pepperoni Pizza", "Quattro Formaggi", "Diavola",
        "Capricciosa", "Hawaiian Pizza", "BBQ Chicken Pizza", "Vegetarian Pizza",
        "Meat Lovers Pizza", "Buffalo Chicken Pizza", "White Pizza", "Sicilian Slice",
        "Neapolitan Pizza", "Prosciutto Arugula Pizza", "Truffle Mushroom Pizza",
        "Four Seasons Pizza", "Calzone", "Stromboli", "Supreme Pizza",
        "Mediterranean Pizza", "Pesto Chicken Pizza", "Spinach Ricotta Pizza",
        "Sausage and Peppers Pizza", "Anchovy Pizza", "Clam Pizza", "Detroit Style Pizza",
        "Chicago Deep Dish", "Marinara Pizza", "Puttanesca Pizza", "Bianca Pizza",
        "Funghi Pizza", "Nduja Pizza", "Roman Pizza al Taglio", "Grandma Pizza",
        "Tomato Basil Pizza", "Artichoke Pizza", "Goat Cheese Pizza", "Spicy Salami Pizza",
        "Tuna Pizza", "Gorgonzola Pear Pizza", "Eggplant Pizza", "Pepperoni Cup Pizza",
        "Vodka Sauce Pizza", "Chicken Bacon Ranch Pizza", "Philly Steak Pizza"
    ];

    private static readonly string[] SushiNames =
    [
        "California Roll", "Spicy Tuna Roll", "Philadelphia Roll", "Dragon Roll",
        "Rainbow Roll", "Spider Roll", "Shrimp Tempura Roll", "Eel Avocado Roll",
        "Salmon Nigiri", "Tuna Nigiri", "Yellowtail Nigiri", "Unagi Nigiri",
        "Sashimi Platter", "Chef's Omakase", "Maki Combo", "Volcano Roll",
        "Caterpillar Roll", "Tiger Roll", "Alaska Roll", "Boston Roll",
        "Futomaki", "Tekka Maki", "Kappa Maki", "Avocado Roll",
        "Hamachi Sashimi", "Octopus Nigiri", "Sea Urchin Nigiri", "Chirashi Bowl",
        "Poke Bowl", "Spicy Salmon Roll", "Crunchy Roll", "Godzilla Roll",
        "Sunset Roll", "Lion King Roll", "Naruto Roll", "Vegetable Roll",
        "Edamame Side", "Seaweed Salad Sushi Set", "Temaki Hand Roll Tuna",
        "Temaki Hand Roll Salmon", "Pressed Sushi Box", "Nigiri Combo Deluxe",
        "Sushi Boat Small", "Sushi Boat Large", "Firecracker Roll", "Spider Deluxe Roll",
        "Soft Shell Crab Roll", "Yellowtail Jalapeño Roll", "Salmon Skin Roll"
    ];

    private static readonly string[] BreakfastNames =
    [
        "Pancakes with Maple Syrup", "French Toast", "Eggs Benedict", "Full English Breakfast",
        "Avocado Toast", "Breakfast Burrito", "Belgian Waffle", "Omelette Station",
        "Huevos Rancheros", "Bagel with Lox", "Croissant Sandwich", "Breakfast Platter",
        "Steel-Cut Oatmeal", "Yogurt Parfait", "Breakfast Tacos", "Shakshuka",
        "Corned Beef Hash", "Biscuits and Gravy", "Smoked Salmon Plate", "Quiche Lorraine",
        "Breakfast Bowl", "Acai Bowl", "Chilaquiles", "Dutch Baby Pancake",
        "Breakfast Sandwich", "Sausage Links Plate", "Crispy Bacon Plate",
        "Egg White Frittata", "Spinach Omelette", "Mushroom Omelette", "Denver Omelette",
        "French Omelette", "Crepes with Nutella", "Crepes with Berries", "Granola Bowl",
        "Breakfast Smoothie Bowl", "Chicken and Waffles", "Steak and Eggs",
        "Smoked Salmon Benedict", "Irish Breakfast", "Continental Breakfast",
        "Breakfast Croissant", "Egg Sandwich", "Breakfast Pizza Slice", "Tofu Scramble Plate",
        "Vegan Pancakes", "Protein Pancakes", "Banana Pancakes", "Blueberry Waffle"
    ];

    private static readonly string[] VeganNames =
    [
        "Quinoa Buddha Bowl", "Tofu Stir-Fry", "Vegan Burger", "Chickpea Curry",
        "Lentil Dal", "Vegan Pad Thai", "Jackfruit Tacos", "Stuffed Bell Peppers",
        "Vegan Ramen", "Mushroom Stroganoff", "Cauliflower Steak", "Vegan Chili",
        "Tempeh Bowl", "Vegan Lasagna", "Zucchini Noodles", "Falafel Plate",
        "Vegan Sushi Roll", "Beyond Bowl", "Sweet Potato Bowl", "Kale Caesar Vegan",
        "Vegan Mac and Cheese", "Eggplant Schnitzel", "Vegan Pho", "Seitan Skewers",
        "Black Bean Burger", "Vegan Pizza Margherita", "Coconut Curry Tofu",
        "Miso Glazed Tofu", "Vegan Burrito", "Roasted Vegetable Plate",
        "Spinach Vegan Lasagna", "Vegan Shepherd's Pie", "Nut Roast Slice",
        "Vegan BLT", "Avocado Sushi Roll", "Tofu Scramble", "Vegan Breakfast Bowl",
        "Chia Pudding", "Acai Vegan Bowl", "Protein Power Vegan Bowl",
        "Mushroom Risotto Vegan", "Pumpkin Coconut Soup", "Vegan Gyoza",
        "Teriyaki Tempeh", "Vegan Cobb Salad", "Cashew Alfredo Pasta",
        "Vegan Tiramisu Slice", "Raw Energy Bowl", "Edamame Noodle Bowl"
    ];

    private static readonly string[] GenericNames =
    [
        "House Special Plate", "Chef's Recommendation", "Seasonal Plate", "Market Fresh Bowl",
        "Classic Combo", "Signature Dish", "Daily Catch", "Farm Plate",
        "Artisan Plate", "Kitchen Favorite", "Guest Favorite", "Homestyle Plate"
    ];

    public static async Task SeedAsync(MenuDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Categories.AnyAsync(cancellationToken))
            return;

        var catFaker = new Faker();
        var categories = new List<Category>(CategoryCount);
        for (var i = 0; i < CategoryCount; i++)
        {
            categories.Add(new Category
            {
                Name = catFaker.PickRandom(RestaurantCategoryNames),
                Description = catFaker.Lorem.Sentence(catFaker.Random.Int(5, 15)),
                SortOrder = i + 1
            });
        }

        await db.Categories.AddRangeAsync(categories, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var allergenPool = new[] { "Milk", "Eggs", "Nuts", "Gluten", "Soy", "Fish", "Shellfish", "Sesame" };

        var dishes = new List<Dish>(CategoryCount * DishesPerCategory);
        var dishFaker = new Faker();
        foreach (var category in categories)
        {
            for (var i = 0; i < DishesPerCategory; i++)
            {
                var allergenCount = dishFaker.Random.Int(0, 3);
                var allergens = Enumerable.Range(0, allergenCount)
                    .Select(_ => dishFaker.PickRandom(allergenPool))
                    .Distinct()
                    .ToList();

                var dishName = BuildDishName(dishFaker, category.Name);

                dishes.Add(new Dish
                {
                    CategoryId = category.Id,
                    Name = dishName,
                    Description = dishFaker.Lorem.Sentence(dishFaker.Random.Int(5, 15)),
                    Price = Math.Round(dishFaker.Random.Decimal(4.5m, 89.99m), 2),
                    IsAvailable = dishFaker.Random.Bool(0.85f),
                    Calories = dishFaker.Random.Int(120, 980),
                    Allergens = allergens
                });
            }
        }

        const int batchSize = 1000;
        for (var i = 0; i < dishes.Count; i += batchSize)
        {
            var batch = dishes.Skip(i).Take(batchSize).ToList();
            await db.Dishes.AddRangeAsync(batch, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        var dishIds = await db.Dishes.AsNoTracking().Select(d => new { d.Id, d.Price }).ToListAsync(cancellationToken);
        var specials = new List<DailySpecial>(TargetDailySpecials);
        var used = new HashSet<(int DishId, DateOnly Date)>();
        var specialFaker = new Faker();

        while (specials.Count < TargetDailySpecials)
        {
            var pick = specialFaker.PickRandom(dishIds);
            var date = DateOnly.FromDateTime(specialFaker.Date.Between(DateTime.UtcNow.AddDays(-400), DateTime.UtcNow.AddDays(30)));
            if (!used.Add((pick.Id, date)))
                continue;

            var discount = specialFaker.Random.Int(1, 50);
            specials.Add(new DailySpecial
            {
                DishId = pick.Id,
                Date = date,
                DiscountPercent = discount,
                SpecialPrice = Math.Round(pick.Price * (1 - discount / 100m), 2)
            });
        }

        for (var i = 0; i < specials.Count; i += batchSize)
        {
            var batch = specials.Skip(i).Take(batchSize).ToList();
            await db.DailySpecials.AddRangeAsync(batch, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    private static string BuildDishName(Faker f, string categoryName) =>
        categoryName switch
        {
            "Appetizers" => f.PickRandom(AppetizerNames),
            "Salads" => f.PickRandom(SaladNames),
            "Soups" => f.PickRandom(SoupNames),
            "Main Courses" => f.PickRandom(MainCourseNames),
            "Desserts" => f.PickRandom(DessertNames),
            "Beverages" => f.PickRandom(BeverageNames),
            "Pizza" => f.PickRandom(PizzaNames),
            "Sushi" => f.PickRandom(SushiNames),
            "Breakfast" => f.PickRandom(BreakfastNames),
            "Vegan" => f.PickRandom(VeganNames),
            _ => f.PickRandom(MainCourseNames.Concat(GenericNames).ToArray())
        };
}
