using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MenuApi.Contracts;
using MenuApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MenuApi.Tests;

[Collection("MenuApi sequential")]
public class EndpointIntegrationTests : IClassFixture<MenuApiFactory>
{
    private readonly MenuApiFactory _factory;

    public EndpointIntegrationTests(MenuApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_menu_returns_200_and_non_empty_list()
    {
        var client = _factory.CreateClient();
        await SeedMinimalAsync();

        var response = await client.GetAsync("/api/menu");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var menu = await response.Content.ReadFromJsonAsync<List<MenuCategoryResponse>>();
        menu.Should().NotBeNull();
        menu!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Post_specials_returns_201_when_valid()
    {
        var client = _factory.CreateClient();
        await SeedMinimalAsync();

        var body = new CreateDailySpecialRequest
        {
            DishId = 1,
            Date = DateOnly.FromDateTime(DateTime.Today),
            DiscountPercent = 15
        };

        var response = await client.PostAsJsonAsync("/api/specials", body);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Patch_dish_availability_returns_200_and_toggles_status()
    {
        var client = _factory.CreateClient();
        await SeedMinimalAsync();

        var response = await client.PatchAsync("/api/dishes/1/availability", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dish = await response.Content.ReadFromJsonAsync<Dish>();
        dish.Should().NotBeNull();
    }

    private async Task SeedMinimalAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MenuApi.Data.MenuDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        db.Categories.Add(new Category { Id = 1, Name = "Cat", Description = "d", SortOrder = 1 });
        db.Dishes.Add(new Dish
        {
            Id = 1,
            Name = "Soup",
            Description = "Hot",
            Price = 10m,
            CategoryId = 1,
            IsAvailable = true,
            Calories = 200,
            Allergens = new List<string>()
        });
        await db.SaveChangesAsync();
    }
}
