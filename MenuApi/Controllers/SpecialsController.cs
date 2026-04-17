using MenuApi.Contracts;
using MenuApi.Data;
using MenuApi.Models;
using MenuApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MenuApi.Controllers;

[ApiController]
[Route("api/specials")]
public class SpecialsController : ControllerBase
{
    private readonly MenuDbContext _db;

    public SpecialsController(MenuDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<ActionResult<DailySpecial>> Create([FromBody] CreateDailySpecialRequest request, CancellationToken cancellationToken)
    {
        try
        {
            MenuBusinessRules.EnsureDiscountPercent(request.DiscountPercent);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }

        var dish = await _db.Dishes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == request.DishId, cancellationToken);
        if (dish is null)
            return NotFound("Dish not found.");

        var exists = await _db.DailySpecials.AnyAsync(
            s => s.DishId == request.DishId && s.Date == request.Date,
            cancellationToken);
        if (exists)
            return Conflict("A special for this dish and date already exists.");

        var specialPrice = Math.Round(MenuBusinessRules.ComputeSpecialPrice(dish.Price, request.DiscountPercent), 2);

        var entity = new DailySpecial
        {
            DishId = request.DishId,
            Date = request.Date,
            DiscountPercent = request.DiscountPercent,
            SpecialPrice = specialPrice
        };
        _db.DailySpecials.Add(entity);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Conflict("A special for this dish and date already exists.");
        }

        return CreatedAtAction(nameof(GetToday), new { id = entity.Id }, entity);
    }

    [HttpGet("today")]
    public async Task<ActionResult<List<DailySpecialResponse>>> GetToday(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var specials = await _db.DailySpecials
            .AsNoTracking()
            .Where(s => s.Date == today)
            .Select(s => new DailySpecialResponse
            {
                Id = s.Id,
                DishId = s.DishId,
                Date = s.Date,
                DiscountPercent = s.DiscountPercent,
                SpecialPrice = s.SpecialPrice
            })
            .ToListAsync(cancellationToken);

        return Ok(specials);
    }
}
