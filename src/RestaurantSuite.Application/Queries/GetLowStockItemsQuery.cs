using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to retrieve only inventory items that are at or below minimum stock level
/// This is useful for generating alerts and reorder reports
/// </summary>
public class GetLowStockItemsQuery : IRequest<List<InventoryItemDto>>
{
}
