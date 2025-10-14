using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.DTOs;

/// <summary>
/// Data transfer object for stock transaction information
/// </summary>
public class StockTransactionDto
{
    public Guid Id { get; set; }
    public Guid InventoryItemId { get; set; }
    public string InventoryItemName { get; set; } = string.Empty;
    public string InventoryItemSKU { get; set; } = string.Empty;
    public StockTransactionType TransactionType { get; set; }
    public string TransactionTypeName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal PreviousStock { get; set; }
    public decimal NewStock { get; set; }
    public Guid? OrderId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
