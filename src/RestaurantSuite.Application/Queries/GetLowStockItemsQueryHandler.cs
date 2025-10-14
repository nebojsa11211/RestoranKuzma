using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

public class GetLowStockItemsQueryHandler : IRequestHandler<GetLowStockItemsQuery, List<InventoryItemDto>>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;

    public GetLowStockItemsQueryHandler(IInventoryItemRepository inventoryItemRepository)
    {
        _inventoryItemRepository = inventoryItemRepository;
    }

    public async Task<List<InventoryItemDto>> Handle(GetLowStockItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _inventoryItemRepository.GetLowStockItemsAsync(cancellationToken);

        return items.Select(item => new InventoryItemDto
        {
            Id = item.Id,
            Name = item.Name,
            SKU = item.SKU,
            CurrentStock = item.CurrentStock,
            Unit = item.Unit,
            MinimumStockLevel = item.MinimumStockLevel,
            UnitCost = item.UnitCost,
            IsActive = item.IsActive,
            IsLowStock = true, // All items in this result are low stock
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        }).ToList();
    }
}
