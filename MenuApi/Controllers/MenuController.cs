using MenuApi.Contracts;
using MenuApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MenuApi.Controllers;

[ApiController]
[Route("api/menu")]
public class MenuController : ControllerBase
{
    private readonly MenuDbContext _db;

    public MenuController(MenuDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<MenuCategoryResponse>>> GetFullMenu(CancellationToken cancellationToken)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.SortOrder)
            .Include(c => c.Dishes)
            .ToListAsync(cancellationToken);

        var result = categories.Select(c => new MenuCategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            SortOrder = c.SortOrder,
            Dishes = c.Dishes.Select(d => new MenuDishResponse
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                Price = d.Price,
                CategoryId = d.CategoryId,
                IsAvailable = d.IsAvailable,
                Calories = d.Calories,
                Allergens = d.Allergens.ToList()
            }).ToList()
        }).ToList();

        return Ok(result);
    }
}
