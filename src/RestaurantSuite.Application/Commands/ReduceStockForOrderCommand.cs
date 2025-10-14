using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to automatically reduce inventory stock based on order items
/// This is triggered when an order starts preparation (InProgress status)
/// </summary>
public class ReduceStockForOrderCommand : IRequest<ReduceStockForOrderResult>
{
    public Guid OrderId { get; set; }
    public Guid ProcessedBy { get; set; }  // User ID of the person processing the order (e.g., chef)
}

public class ReduceStockForOrderResult
{
    public bool Success { get; set; }
    public List<string> Messages { get; set; } = new();
    public List<StockReductionDetail> Reductions { get; set; } = new();
}

public class StockReductionDetail
{
    public Guid InventoryItemId { get; set; }
    public string InventoryItemName { get; set; } = string.Empty;
    public decimal QuantityReduced { get; set; }
    public decimal PreviousStock { get; set; }
    public decimal NewStock { get; set; }
}
