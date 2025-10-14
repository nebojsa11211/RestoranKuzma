using MediatR;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Commands;

public class CreateInventoryItemCommandHandler : IRequestHandler<CreateInventoryItemCommand, Guid>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryItemCommandHandler(
        IInventoryItemRepository inventoryItemRepository,
        IUnitOfWork unitOfWork)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        // Check if SKU already exists
        var existingItem = await _inventoryItemRepository.GetBySKUAsync(request.SKU, cancellationToken);
        if (existingItem != null)
            throw new InvalidOperationException($"An inventory item with SKU '{request.SKU}' already exists");

        var inventoryItem = InventoryItem.Create(
            request.Name,
            request.Description,
            request.SKU,
            request.InitialStock,
            request.Unit,
            request.MinimumStockLevel,
            request.UnitCost);

        await _inventoryItemRepository.AddAsync(inventoryItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return inventoryItem.Id;
    }
}
