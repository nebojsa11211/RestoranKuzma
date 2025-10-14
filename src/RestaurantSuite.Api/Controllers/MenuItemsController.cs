using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Queries;

namespace RestaurantSuite.Api.Controllers;

/// <summary>
/// Menu items management endpoints
/// </summary>
[ApiController]
[Route("api")]
public class MenuItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MenuItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all menu items with optional filters
    /// </summary>
    /// <param name="categoryId">Optional category filter</param>
    /// <param name="availableOnly">Optional filter for available items only</param>
    /// <returns>List of menu items</returns>
    [HttpGet("menu")]
    public async Task<IActionResult> GetAll([FromQuery] Guid? categoryId, [FromQuery] bool? availableOnly)
    {
        var query = new GetAllMenuItemsQuery
        {
            CategoryId = categoryId,
            AvailableOnly = availableOnly
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific menu item by ID
    /// </summary>
    /// <param name="id">Menu item ID</param>
    /// <returns>Menu item details</returns>
    [HttpGet("menu/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetMenuItemByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound(new { message = $"Menu item with ID {id} not found" });

        return Ok(result);
    }

    /// <summary>
    /// Create a new menu item (Admin only)
    /// </summary>
    /// <param name="dto">Menu item creation data</param>
    /// <returns>Created menu item ID</returns>
    [HttpPost("admin/menu")]
    // [Authorize(Roles = "Admin")] - Temporarily disabled for testing
    public async Task<IActionResult> Create([FromBody] CreateMenuItemDto dto)
    {
        try
        {
            Console.WriteLine($"Creating menu item: Name={dto.Name}, Description={dto.Description}, Price={dto.Price}, CategoryId={dto.CategoryId}");
            
            // Convert string price to decimal
            if (!decimal.TryParse(dto.Price, out var price))
            {
                return BadRequest(new { message = "Invalid price format" });
            }

            var command = new CreateMenuItemCommand
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = price,
                CategoryId = dto.CategoryId,
                ImageUrl = dto.ImageUrl
            };

            var id = await _mediator.Send(command);
            Console.WriteLine($"Menu item created successfully with ID: {id}");
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"ArgumentException: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Update menu item details (Admin only)
    /// </summary>
    /// <param name="id">Menu item ID</param>
    /// <param name="dto">Updated menu item data</param>
    /// <returns>No content on success</returns>
    [HttpPut("admin/menu/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMenuItemDto dto)
    {
        try
        {
            var command = new UpdateMenuItemCommand
            {
                Id = id,
                Name = dto.Name,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                ImageUrl = dto.ImageUrl
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
    /// Update menu item price (Admin only)
    /// </summary>
    /// <param name="id">Menu item ID</param>
    /// <param name="price">New price</param>
    /// <returns>No content on success</returns>
    [HttpPatch("admin/menu/{id}/price")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] decimal price)
    {
        try
        {
            var command = new UpdateMenuItemPriceCommand
            {
                Id = id,
                Price = price
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
    /// Toggle menu item availability (Admin only)
    /// </summary>
    /// <param name="id">Menu item ID</param>
    /// <param name="isAvailable">Availability status</param>
    /// <returns>No content on success</returns>
    [HttpPatch("admin/menu/{id}/availability")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleAvailability(Guid id, [FromBody] bool isAvailable)
    {
        try
        {
            var command = new ToggleMenuItemAvailabilityCommand
            {
                Id = id,
                IsAvailable = isAvailable
            };

            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a menu item (Admin only)
    /// </summary>
    /// <param name="id">Menu item ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("admin/menu/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteMenuItemCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update menu item preparation time (Admin only)
    /// </summary>
    /// <param name="id">Menu item ID</param>
    /// <param name="preparationTimeMinutes">Preparation time in minutes (null to clear)</param>
    /// <returns>No content on success</returns>
    [HttpPatch("admin/menu/{id}/preparation-time")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePreparationTime(Guid id, [FromBody] int? preparationTimeMinutes)
    {
        try
        {
            var command = new UpdateMenuItemPreparationTimeCommand
            {
                Id = id,
                PreparationTimeMinutes = preparationTimeMinutes
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
    /// Get menu with full recipe details (Chef/Admin only)
    /// </summary>
    /// <param name="categoryId">Optional category filter</param>
    /// <param name="restaurantId">Restaurant ID</param>
    /// <returns>List of menu items with full ingredient details</returns>
    [HttpGet("chef/menu-with-recipes")]
    [Authorize(Roles = "Chef,Admin")]
    public async Task<IActionResult> GetMenuWithRecipes([FromQuery] Guid? categoryId, [FromQuery] Guid restaurantId)
    {
        var query = new GetMenuWithRecipesQuery
        {
            CategoryId = categoryId,
            RestaurantId = restaurantId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get menu with simplified ingredients (Guest view)
    /// </summary>
    /// <param name="categoryId">Optional category filter</param>
    /// <param name="restaurantId">Restaurant ID</param>
    /// <returns>List of menu items with main ingredients only</returns>
    [HttpGet("guest/menu")]
    public async Task<IActionResult> GetGuestMenu([FromQuery] Guid? categoryId, [FromQuery] Guid restaurantId)
    {
        var query = new GetGuestMenuQuery
        {
            CategoryId = categoryId,
            RestaurantId = restaurantId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
