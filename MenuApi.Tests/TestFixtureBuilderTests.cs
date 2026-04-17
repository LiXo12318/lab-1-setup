using AutoFixture;
using FluentAssertions;
using MenuApi.Models;

namespace MenuApi.Tests;

public class TestFixtureBuilderTests
{
    [Fact]
    public void Create_generates_only_configured_string_and_collection_fields_for_entities()
    {
        var fixture = TestFixtureBuilder.Create();

        var category = fixture.Create<Category>();
        category.Name.Should().NotBeNullOrWhiteSpace();
        category.Description.Should().NotBeNullOrWhiteSpace();
        category.SortOrder.Should().Be(0);
        category.Id.Should().Be(0);
        category.Dishes.Should().BeEmpty();

        var dish = fixture.Create<Dish>();
        dish.Name.Should().NotBeNullOrWhiteSpace();
        dish.Description.Should().NotBeNullOrWhiteSpace();
        dish.Allergens.Should().NotBeNull();
        dish.Price.Should().Be(0);
        dish.CategoryId.Should().Be(0);
        dish.IsAvailable.Should().BeFalse();
        dish.Calories.Should().Be(0);

        var special = fixture.Create<DailySpecial>();
        special.Date.Should().NotBe(default(DateOnly));
        special.DiscountPercent.Should().Be(0);
        special.SpecialPrice.Should().Be(0);
        special.DishId.Should().Be(0);
    }
}
