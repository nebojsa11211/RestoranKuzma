using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Queries;

namespace RestaurantSuite.Api.Controllers;

/// <summary>
/// Menu item ingredients (recipe) management endpoints
/// </summary>
[ApiController]
[Route("api")]
public class MenuItemIngredientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MenuItemIngredientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get menu item with complete recipe (Admin/Chef only)
    /// </summary>
    /// <param name="id">Menu item ID</param>
    /// <returns>Menu item with full ingredient list and quantities</returns>
    [HttpGet("admin/menu/{id}/recipe")]
    [Authorize(Roles = "Admin,Chef")]
    public async Task<IActionResult> GetMenuItemWithRecipe(Guid id)
    {
        var query = new GetMenuItemWithIngredientsQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound(new { message = $"Menu item with ID {id} not found" });

        return Ok(result);
    }

    /// <summary>
    /// Get ingredients for a menu item (Admin/Chef only)
    /// </summary>
    /// <param name="menuItemId">Menu item ID</param>
    /// <returns>List of ingredients with quantities</returns>
    [HttpGet("admin/menu/{menuItemId}/ingredients")]
    [Authorize(Roles = "Admin,Chef")]
    public async Task<IActionResult> GetIngredients(Guid menuItemId)
    {
        var query = new GetMenuItemIngredientsQuery { MenuItemId = menuItemId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Add an ingredient to a menu item (Admin only)
    /// </summary>
    /// <param name="menuItemId">Menu item ID</param>
    /// <param name="dto">Ingredient data</param>
    /// <returns>Created ingredient ID</returns>
    [HttpPost("admin/menu/{menuItemId}/ingredients")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddIngredient(Guid menuItemId, [FromBody] CreateMenuItemIngredientDto dto)
    {
        try
        {
            var command = new AddMenuItemIngredientCommand
            {
                MenuItemId = menuItemId,
                IngredientName = dto.IngredientName,
                QuantityInGrams = dto.QuantityInGrams,
                IsMainIngredient = dto.IsMainIngredient,
                DisplayOrder = dto.DisplayOrder
            };

            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetIngredients), new { menuItemId }, new { id });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an ingredient (Admin only)
    /// </summary>
    /// <param name="id">Ingredient ID</param>
    /// <param name="dto">Updated ingredient data</param>
    /// <returns>No content on success</returns>
    [HttpPut("admin/ingredients/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateIngredient(Guid id, [FromBody] UpdateMenuItemIngredientDto dto)
    {
        try
        {
            var command = new UpdateMenuItemIngredientCommand
            {
                Id = id,
                IngredientName = dto.IngredientName,
                QuantityInGrams = dto.QuantityInGrams,
                IsMainIngredient = dto.IsMainIngredient,
                DisplayOrder = dto.DisplayOrder
            };

            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete an ingredient (Admin only)
    /// </summary>
    /// <param name="id">Ingredient ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("admin/ingredients/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteIngredient(Guid id)
    {
        try
        {
            var command = new DeleteMenuItemIngredientCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
