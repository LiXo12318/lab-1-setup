using MenuApi.Models;

namespace MenuApi.Services;

public static class DishAllergenFilter
{
    public static List<Dish> ExcludeDishesWithAllergens(
        IReadOnlyList<Dish> dishes,
        IReadOnlyCollection<string>? allergensToExclude)
    {
        if (allergensToExclude is null || allergensToExclude.Count == 0)
            return dishes.ToList();

        var excludeSet = allergensToExclude
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Select(a => a.Trim())
            .ToHashSet(StringComparer.Ordinal);

        if (excludeSet.Count == 0)
            return dishes.ToList();

        return dishes
            .Where(d => !d.Allergens.Any(a => excludeSet.Contains(a)))
            .ToList();
    }
}
