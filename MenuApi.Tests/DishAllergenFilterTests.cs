using FluentAssertions;
using MenuApi.Models;
using MenuApi.Services;

namespace MenuApi.Tests;

public class DishAllergenFilterTests
{
    private static List<Dish> SampleDishes() =>
    [
        new Dish { Id = 1, Name = "A", Allergens = ["Milk", "Eggs"] },
        new Dish { Id = 2, Name = "B", Allergens = ["Fish"] },
        new Dish { Id = 3, Name = "C", Allergens = [] }
    ];

    [Fact]
    public void ExcludeDishesWithAllergens_returns_all_when_no_exclusions()
    {
        var dishes = SampleDishes();
        var result = DishAllergenFilter.ExcludeDishesWithAllergens(dishes, null);
        result.Should().HaveCount(3);
    }

    [Fact]
    public void ExcludeDishesWithAllergens_removes_dishes_containing_any_listed_allergen()
    {
        var dishes = SampleDishes();
        var result = DishAllergenFilter.ExcludeDishesWithAllergens(dishes, ["Milk"]);
        result.Should().HaveCount(2);
        result.Select(d => d.Id).Should().BeEquivalentTo([2, 3]);
    }

    [Fact]
    public void ExcludeDishesWithAllergens_trims_and_ignores_whitespace_entries()
    {
        var dishes = SampleDishes();
        var result = DishAllergenFilter.ExcludeDishesWithAllergens(dishes, ["  Milk  ", " "]);
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ExcludeDishesWithAllergens_excludes_when_multiple_allergens_match()
    {
        var dishes = SampleDishes();
        var result = DishAllergenFilter.ExcludeDishesWithAllergens(dishes, ["Milk", "Fish"]);
        result.Should().ContainSingle(d => d.Id == 3);
    }
}
