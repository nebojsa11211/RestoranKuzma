using MediatR;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.Commands;

public class CreateInventoryItemCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal InitialStock { get; set; }
    public UnitOfMeasure Unit { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal UnitCost { get; set; }
}
