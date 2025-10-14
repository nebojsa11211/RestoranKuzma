using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

public class GetInventoryItemByIdQueryHandler : IRequestHandler<GetInventoryItemByIdQuery, InventoryItemDto?>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;

    public GetInventoryItemByIdQueryHandler(IInventoryItemRepository inventoryItemRepository)
    {
        _inventoryItemRepository = inventoryItemRepository;
    }

    public async Task<InventoryItemDto?> Handle(GetInventoryItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _inventoryItemRepository.GetByIdAsync(request.Id, cancellationToken);

        if (item == null)
            return null;

        return new InventoryItemDto
        {
            Id = item.Id,
            Name = item.Name,
            SKU = item.SKU,
            CurrentStock = item.CurrentStock,
            Unit = item.Unit,
            MinimumStockLevel = item.MinimumStockLevel,
            UnitCost = item.UnitCost,
            IsActive = item.IsActive,
            IsLowStock = item.IsLowStock(),
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
