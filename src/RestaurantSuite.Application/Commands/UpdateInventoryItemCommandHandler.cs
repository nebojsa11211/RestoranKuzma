using MediatR;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Commands;

public class UpdateInventoryItemCommandHandler : IRequestHandler<UpdateInventoryItemCommand, Unit>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInventoryItemCommandHandler(
        IInventoryItemRepository inventoryItemRepository,
        IUnitOfWork unitOfWork)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var inventoryItem = await _inventoryItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (inventoryItem == null)
            throw new InvalidOperationException($"Inventory item {request.Id} not found");

        // Check if SKU is being changed and if new SKU already exists
        if (inventoryItem.SKU != request.SKU.ToUpperInvariant())
        {
            var existingItem = await _inventoryItemRepository.GetBySKUAsync(request.SKU, cancellationToken);
            if (existingItem != null && existingItem.Id != request.Id)
                throw new InvalidOperationException($"An inventory item with SKU '{request.SKU}' already exists");
        }

        inventoryItem.UpdateDetails(
            request.Name,
            request.Description,
            request.SKU,
            request.Unit,
            request.MinimumStockLevel,
            request.UnitCost);

        // Handle active status if specified
        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
                inventoryItem.Activate();
            else
                inventoryItem.Deactivate();
        }

        _inventoryItemRepository.Update(inventoryItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
