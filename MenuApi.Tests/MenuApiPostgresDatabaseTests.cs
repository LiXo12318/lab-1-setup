using FluentAssertions;
using MenuApi.Data;
using MenuApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MenuApi.Tests;

[CollectionDefinition("Postgres sequential", DisableParallelization = true)]
public class PostgresSequentialCollection;

[Collection("Postgres sequential")]
public class MenuApiPostgresDatabaseTests : IClassFixture<PostgreSqlContainerFixture>
{
    private readonly PostgreSqlContainerFixture _postgres;

    private static readonly SemaphoreSlim SchemaGate = new(1, 1);
    private static bool _schemaReady;

    public MenuApiPostgresDatabaseTests(PostgreSqlContainerFixture postgres)
    {
        _postgres = postgres;
    }

    private MenuDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MenuDbContext>()
            .UseNpgsql(_postgres.ConnectionString)
            .Options;
        return new MenuDbContext(options);
    }

    private static async Task PrepareCleanDatabaseAsync(MenuDbContext db)
    {
        await SchemaGate.WaitAsync();
        try
        {
            if (!_schemaReady)
            {
                await db.Database.MigrateAsync();
                _schemaReady = true;
                return;
            }

            await db.Database.ExecuteSqlRawAsync(
                """
                TRUNCATE TABLE "DailySpecials" RESTART IDENTITY CASCADE;
                TRUNCATE TABLE "Dishes" RESTART IDENTITY CASCADE;
                TRUNCATE TABLE "Categories" RESTART IDENTITY CASCADE;
                """);
        }
        finally
        {
            SchemaGate.Release();
        }
    }

    [Fact]
    public async Task DailySpecial_duplicate_dish_and_date_throws_DbUpdateException()
    {
        await using var db = CreateContext();
        await PrepareCleanDatabaseAsync(db);

        var category = new Category { Name = "C", Description = "d", SortOrder = 1 };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var dish = new Dish
        {
            Name = "D",
            Description = "x",
            Price = 10m,
            CategoryId = category.Id,
            IsAvailable = true,
            Calories = 100,
            Allergens = new List<string>()
        };
        db.Dishes.Add(dish);
        await db.SaveChangesAsync();

        var date = new DateOnly(2026, 6, 1);
        db.DailySpecials.Add(new DailySpecial
        {
            DishId = dish.Id,
            Date = date,
            DiscountPercent = 10,
            SpecialPrice = 9m
        });
        await db.SaveChangesAsync();

        db.DailySpecials.Add(new DailySpecial
        {
            DishId = dish.Id,
            Date = date,
            DiscountPercent = 20,
            SpecialPrice = 8m
        });

        var act = async () => await db.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Categories_ordered_by_SortOrder_match_inserted_sequence()
    {
        await using var db = CreateContext();
        await PrepareCleanDatabaseAsync(db);

        db.Categories.AddRange(
            new Category { Name = "Third", Description = "a", SortOrder = 30 },
            new Category { Name = "First", Description = "b", SortOrder = 5 },
            new Category { Name = "Second", Description = "c", SortOrder = 15 });
        await db.SaveChangesAsync();

        var ordered = await db.Categories.AsNoTracking()
            .OrderBy(c => c.SortOrder)
            .Select(c => c.Name)
            .ToListAsync();

        ordered.Should().Equal("First", "Second", "Third");
    }

    [Fact]
    public async Task Dish_with_invalid_CategoryId_throws_DbUpdateException()
    {
        await using var db = CreateContext();
        await PrepareCleanDatabaseAsync(db);

        db.Dishes.Add(new Dish
        {
            Name = "Orphan",
            Description = "x",
            Price = 5m,
            CategoryId = 42_424,
            IsAvailable = true,
            Calories = 50,
            Allergens = new List<string>()
        });

        var act = async () => await db.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }
}
