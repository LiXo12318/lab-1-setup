using AutoFixture;
using MenuApi.Models;

namespace MenuApi.Tests;

public static class TestFixtureBuilder
{
    public static Fixture Create()
    {
        var fixture = new Fixture();

        fixture.Customize<Category>(c => c
            .OmitAutoProperties()
            .With(x => x.Name)
            .With(x => x.Description));

        fixture.Customize<Dish>(c => c
            .OmitAutoProperties()
            .With(x => x.Name)
            .With(x => x.Description)
            .With(x => x.Allergens));

        fixture.Customize<DailySpecial>(c => c
            .OmitAutoProperties()
            .With(x => x.Date, () => DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(Random.Shared.Next(-400, 400)))));

        return fixture;
    }
}

