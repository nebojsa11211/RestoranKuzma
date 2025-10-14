using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.Queries;

namespace RestaurantSuite.Api.Controllers;

/// <summary>
/// Recipe management endpoints - linking menu items to inventory ingredients
/// </summary>
[ApiController]
[Route("api/admin/recipes")]
[Authorize(Roles = "Admin,Chef")]
public class RecipesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecipesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all recipes with their ingredients
    /// </summary>
    /// <param name="includeInactive">Include inactive recipes</param>
    /// <returns>List of recipes</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var query = new GetAllRecipesQuery { IncludeInactive = includeInactive };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get recipe for a specific menu item
    /// </summary>
    /// <param name="menuItemId">Menu item ID</param>
    /// <returns>Recipe details with ingredients</returns>
    [HttpGet("menu-item/{menuItemId}")]
    public async Task<IActionResult> GetByMenuItemId(Guid menuItemId)
    {
        var query = new GetRecipeByMenuItemIdQuery { MenuItemId = menuItemId };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound(new { message = $"No recipe found for menu item with ID {menuItemId}" });

        return Ok(result);
    }

    /// <summary>
    /// Create a new recipe for a menu item (Admin only)
    /// </summary>
    /// <param name="command">Recipe creation data with ingredients</param>
    /// <returns>Created recipe ID</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateRecipeCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetByMenuItemId), new { menuItemId = command.MenuItemId }, new { id });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update a recipe's ingredients (Admin only)
    /// </summary>
    /// <param name="menuItemId">Menu item ID</param>
    /// <param name="command">Updated recipe data</param>
    /// <returns>No content on success</returns>
    [HttpPut("menu-item/{menuItemId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid menuItemId, [FromBody] UpdateRecipeCommand command)
    {
        try
        {
            command.MenuItemId = menuItemId;
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Deactivate a recipe (soft delete) (Admin only)
    /// </summary>
    /// <param name="menuItemId">Menu item ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("menu-item/{menuItemId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(Guid menuItemId)
    {
        try
        {
            var command = new DeactivateRecipeCommand { MenuItemId = menuItemId };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
