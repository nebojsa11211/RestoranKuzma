using MediatR;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Commands;

public class AdjustInventoryStockCommandHandler : IRequestHandler<AdjustInventoryStockCommand, Unit>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdjustInventoryStockCommandHandler(
        IInventoryItemRepository inventoryItemRepository,
        IStockTransactionRepository stockTransactionRepository,
        IUnitOfWork unitOfWork)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _stockTransactionRepository = stockTransactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AdjustInventoryStockCommand request, CancellationToken cancellationToken)
    {
        var inventoryItem = await _inventoryItemRepository.GetByIdAsync(request.InventoryItemId, cancellationToken);
        if (inventoryItem == null)
            throw new InvalidOperationException($"Inventory item {request.InventoryItemId} not found");

        var previousStock = inventoryItem.CurrentStock;

        // Adjust the stock
        inventoryItem.AdjustStock(request.Quantity);

        // Create transaction record
        var transaction = StockTransaction.Create(
            inventoryItem.Id,
            request.TransactionType,
            request.Quantity,
            previousStock,
            inventoryItem.CurrentStock,
            request.CreatedBy,
            null, // No order associated with manual adjustments
            request.Notes);

        _inventoryItemRepository.Update(inventoryItem);
        await _stockTransactionRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
