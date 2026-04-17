using System.Net;
using System.Net.Http.Json;
using MenuApi.Contracts;
using MenuApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MenuApi.Tests;

[CollectionDefinition("MenuApi sequential", DisableParallelization = true)]
public class MenuApiSequentialCollection;

[Collection("MenuApi sequential")]
public class MenuApiIntegrationTests : IClassFixture<MenuApiFactory>
{
    private readonly MenuApiFactory _factory;

    public MenuApiIntegrationTests(MenuApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Menu_is_grouped_by_category_sorted_by_SortOrder()
    {
        var client = _factory.CreateClient();
        await SeedMinimalMenuAsync();

        var menu = await client.GetFromJsonAsync<List<MenuCategoryResponse>>("/api/menu");
        Assert.NotNull(menu);
        Assert.Equal(2, menu.Count);
        Assert.True(menu[0].SortOrder <= menu[1].SortOrder);
        Assert.NotEmpty(menu[0].Dishes);
    }

    [Fact]
    public async Task Get_dishes_filters_by_category_availability_and_excludes_allergens()
    {
        var client = _factory.CreateClient();
        await SeedMinimalMenuAsync();

        var dishes = await client.GetFromJsonAsync<List<Dish>>("/api/dishes?categoryId=1&isAvailable=true&allergens=Milk");
        Assert.NotNull(dishes);
        Assert.Contains(dishes, d => d.CategoryId == 1 && d.IsAvailable);
        Assert.All(dishes, d => Assert.DoesNotContain("Milk", d.Allergens));
    }

    [Fact]
    public async Task Patch_availability_toggles_IsAvailable()
    {
        var client = _factory.CreateClient();
        await SeedMinimalMenuAsync();

        var before = await client.GetFromJsonAsync<List<Dish>>("/api/dishes");
        var dish = before!.First(d => d.Id == 1);
        var expected = !dish.IsAvailable;

        var response = await client.PatchAsync("/api/dishes/1/availability", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await response.Content.ReadFromJsonAsync<Dish>();
        Assert.NotNull(updated);
        Assert.Equal(expected, updated.IsAvailable);
    }

    [Fact]
    public async Task Post_special_computes_SpecialPrice_and_conflict_on_duplicate()
    {
        var client = _factory.CreateClient();
        await SeedMinimalMenuAsync();

        var date = DateOnly.FromDateTime(DateTime.Today);
        var create = new CreateDailySpecialRequest { DishId = 1, Date = date, DiscountPercent = 10 };
        var ok = await client.PostAsJsonAsync("/api/specials", create);
        Assert.Equal(HttpStatusCode.Created, ok.StatusCode);
        var special = await ok.Content.ReadFromJsonAsync<DailySpecial>();
        Assert.NotNull(special);
        Assert.Equal(9m, special.SpecialPrice);

        var conflict = await client.PostAsJsonAsync("/api/specials", create);
        Assert.Equal(HttpStatusCode.Conflict, conflict.StatusCode);
    }

    private async Task SeedMinimalMenuAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MenuApi.Data.MenuDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        db.Categories.AddRange(
            new Category { Id = 1, Name = "A", Description = "a", SortOrder = 1 },
            new Category { Id = 2, Name = "B", Description = "b", SortOrder = 2 });
        db.Dishes.AddRange(
            new Dish
            {
                Id = 1,
                Name = "Soup",
                Description = "Hot",
                Price = 10m,
                CategoryId = 1,
                IsAvailable = true,
                Calories = 200,
                Allergens = new List<string> { "Fish" }
            },
            new Dish
            {
                Id = 2,
                Name = "Milkshake",
                Description = "Cold",
                Price = 5m,
                CategoryId = 1,
                IsAvailable = true,
                Calories = 400,
                Allergens = new List<string> { "Milk" }
            });
        await db.SaveChangesAsync();
    }
}
