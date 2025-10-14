using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Domain.Entities;

public class StockTransaction
{
    public Guid Id { get; private set; }
    public Guid InventoryItemId { get; private set; }
    public StockTransactionType TransactionType { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal PreviousStock { get; private set; }
    public decimal NewStock { get; private set; }
    public Guid? OrderId { get; private set; }
    public string Notes { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public InventoryItem InventoryItem { get; private set; }
    public Order? Order { get; private set; }
    public User CreatedByUser { get; private set; }

    private StockTransaction() { }

    public static StockTransaction Create(
        Guid inventoryItemId,
        StockTransactionType transactionType,
        decimal quantity,
        decimal previousStock,
        decimal newStock,
        Guid createdBy,
        Guid? orderId = null,
        string? notes = null)
    {
        return new StockTransaction
        {
            Id = Guid.NewGuid(),
            InventoryItemId = inventoryItemId,
            TransactionType = transactionType,
            Quantity = quantity,
            PreviousStock = previousStock,
            NewStock = newStock,
            OrderId = orderId,
            Notes = notes ?? string.Empty,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }
}
