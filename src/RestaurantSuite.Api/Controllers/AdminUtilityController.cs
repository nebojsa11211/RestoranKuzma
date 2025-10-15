using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Infrastructure.EF;

namespace RestaurantSuite.Api.Controllers;

/// <summary>
/// Admin utility endpoints for database management
/// </summary>
[ApiController]
[Route("api/admin/utility")]
public class AdminUtilityController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminUtilityController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seed the database with initial pizza data
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("seed-database")]
    public async Task<IActionResult> SeedDatabase()
    {
        try
        {
            var seeder = new DatabaseSeeder(_context);
            await seeder.SeedAsync();

            return Ok(new {
                message = "Database seeded successfully!",
                details = "Added Pizza category with 15 items, inventory items, and recipes"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {
                message = "Error seeding database",
                error = ex.Message,
                stackTrace = ex.StackTrace
            });
        }
    }
}
