using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.Queries;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Api.Controllers;

/// <summary>
/// Inventory management endpoints
/// </summary>
[ApiController]
[Route("api/admin/inventory")]
[Authorize(Roles = "Admin")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all inventory items with optional filters
    /// </summary>
    /// <param name="lowStockOnly">Filter for low stock items only</param>
    /// <param name="includeInactive">Include inactive items</param>
    /// <returns>List of inventory items</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? lowStockOnly, [FromQuery] bool includeInactive = false)
    {
        var query = new GetAllInventoryItemsQuery
        {
            LowStockOnly = lowStockOnly,
            IncludeInactive = includeInactive
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific inventory item by ID
    /// </summary>
    /// <param name="id">Inventory item ID</param>
    /// <returns>Inventory item details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetInventoryItemByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound(new { message = $"Inventory item with ID {id} not found" });

        return Ok(result);
    }

    /// <summary>
    /// Get inventory items that are at or below minimum stock level
    /// </summary>
    /// <returns>List of low stock items</returns>
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        var query = new GetLowStockItemsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get stock transaction history
    /// </summary>
    /// <param name="inventoryItemId">Filter by inventory item</param>
    /// <param name="orderId">Filter by order</param>
    /// <param name="fromDate">Filter by date range - start</param>
    /// <param name="toDate">Filter by date range - end</param>
    /// <param name="limit">Maximum number of results</param>
    /// <returns>List of stock transactions</returns>
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactionHistory(
        [FromQuery] Guid? inventoryItemId,
        [FromQuery] Guid? orderId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int limit = 100)
    {
        var query = new GetStockTransactionHistoryQuery
        {
            InventoryItemId = inventoryItemId,
            OrderId = orderId,
            FromDate = fromDate,
            ToDate = toDate,
            Limit = limit
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new inventory item
    /// </summary>
    /// <param name="command">Inventory item creation data</param>
    /// <returns>Created inventory item ID</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInventoryItemCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
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
    /// Update inventory item details
    /// </summary>
    /// <param name="id">Inventory item ID</param>
    /// <param name="command">Updated inventory item data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInventoryItemCommand command)
    {
        try
        {
            command.Id = id;
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
    /// Adjust inventory stock (manual adjustment, purchase, wastage, etc.)
    /// </summary>
    /// <param name="id">Inventory item ID</param>
    /// <param name="command">Stock adjustment data</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/adjust")]
    public async Task<IActionResult> AdjustStock(Guid id, [FromBody] AdjustInventoryStockCommand command)
    {
        try
        {
            command.InventoryItemId = id;

            // TODO: Get actual user ID from authentication context
            if (command.CreatedBy == Guid.Empty)
            {
                command.CreatedBy = Guid.Empty; // Placeholder
            }

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
    /// Deactivate an inventory item (soft delete)
    /// </summary>
    /// <param name="id">Inventory item ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            var command = new UpdateInventoryItemCommand { Id = id };
            // Fetch current item to preserve other fields
            var current = await _mediator.Send(new GetInventoryItemByIdQuery { Id = id });

            if (current == null)
                return NotFound(new { message = $"Inventory item with ID {id} not found" });

            command.Name = current.Name;
            command.SKU = current.SKU;
            command.MinimumStockLevel = current.MinimumStockLevel;
            command.UnitCost = current.UnitCost;
            command.IsActive = false;

            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
