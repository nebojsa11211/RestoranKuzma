using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to retrieve a single inventory item by ID
/// </summary>
public class GetInventoryItemByIdQuery : IRequest<InventoryItemDto?>
{
    public Guid Id { get; set; }
}
