using MenuApi.Contracts;
using MenuApi.Data;
using MenuApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MenuApi.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly MenuDbContext _db;

    public CategoriesController(MenuDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Category>>> GetAll(CancellationToken cancellationToken)
    {
        return await _db.Categories.AsNoTracking().OrderBy(c => c.SortOrder).ToListAsync(cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var entity = new Category
        {
            Name = request.Name,
            Description = request.Description,
            SortOrder = request.SortOrder
        };
        _db.Categories.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, entity);
    }
}
