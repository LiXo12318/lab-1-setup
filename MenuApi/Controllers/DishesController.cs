using MenuApi.Contracts;
using MenuApi.Data;
using MenuApi.Models;
using MenuApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MenuApi.Controllers;

[ApiController]
[Route("api/dishes")]
public class DishesController : ControllerBase
{
    private readonly MenuDbContext _db;

    public DishesController(MenuDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Dish>>> GetFiltered(
        [FromQuery] int? categoryId,
        [FromQuery] bool? isAvailable,
        [FromQuery] string[]? allergens,
        CancellationToken cancellationToken)
    {
        var query = _db.Dishes.AsNoTracking().AsQueryable();

        if (categoryId is { } cid)
            query = query.Where(d => d.CategoryId == cid);

        if (isAvailable.HasValue)
            query = query.Where(d => d.IsAvailable == isAvailable.Value);

        var list = await query.ToListAsync(cancellationToken);

        var exclude = allergens?
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Select(a => a.Trim())
            .ToList();

        return DishAllergenFilter.ExcludeDishesWithAllergens(list, exclude);
    }

    [HttpPost]
    public async Task<ActionResult<Dish>> Create([FromBody] CreateDishRequest request, CancellationToken cancellationToken)
    {
        try
        {
            MenuBusinessRules.EnsurePositivePrice(request.Price);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }

        var entity = new Dish
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
            IsAvailable = request.IsAvailable,
            Calories = request.Calories,
            Allergens = request.Allergens ?? new List<string>()
        };
        _db.Dishes.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetFiltered), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Dish>> Update(int id, [FromBody] UpdateDishRequest request, CancellationToken cancellationToken)
    {
        try
        {
            MenuBusinessRules.EnsurePositivePrice(request.Price);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }

        var entity = await _db.Dishes.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (entity is null)
            return NotFound();

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.CategoryId = request.CategoryId;
        entity.IsAvailable = request.IsAvailable;
        entity.Calories = request.Calories;
        entity.Allergens = request.Allergens ?? new List<string>();

        await _db.SaveChangesAsync(cancellationToken);
        return Ok(entity);
    }

    [HttpPatch("{id:int}/availability")]
    public async Task<ActionResult<Dish>> ToggleAvailability(int id, CancellationToken cancellationToken)
    {
        var entity = await _db.Dishes.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (entity is null)
            return NotFound();

        entity.IsAvailable = !entity.IsAvailable;
        await _db.SaveChangesAsync(cancellationToken);
        return Ok(entity);
    }
}
