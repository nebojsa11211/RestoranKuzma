using MediatR;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.Commands;

public class UpdateInventoryItemCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public UnitOfMeasure Unit { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal UnitCost { get; set; }
    public bool? IsActive { get; set; } // Optional - only set if you want to change active status
}
