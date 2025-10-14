using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

public class GetAllInventoryItemsQueryHandler : IRequestHandler<GetAllInventoryItemsQuery, List<InventoryItemDto>>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;

    public GetAllInventoryItemsQueryHandler(IInventoryItemRepository inventoryItemRepository)
    {
        _inventoryItemRepository = inventoryItemRepository;
    }

    public async Task<List<InventoryItemDto>> Handle(GetAllInventoryItemsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.InventoryItem> items;

        if (request.LowStockOnly == true)
        {
            items = await _inventoryItemRepository.GetLowStockItemsAsync(cancellationToken);
        }
        else
        {
            items = await _inventoryItemRepository.GetAllAsync(cancellationToken);
        }

        // Filter by active status if needed
        if (!request.IncludeInactive)
        {
            items = items.Where(i => i.IsActive);
        }

        // Map to DTOs
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
            IsLowStock = item.IsLowStock(),
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        }).ToList();
    }
}
