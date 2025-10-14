using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to retrieve all inventory items with optional filtering
/// </summary>
public class GetAllInventoryItemsQuery : IRequest<List<InventoryItemDto>>
{
    /// <summary>
    /// Filter by low stock items only (optional)
    /// </summary>
    public bool? LowStockOnly { get; set; }

    /// <summary>
    /// Include inactive items (default: false)
    /// </summary>
    public bool IncludeInactive { get; set; } = false;
}
