using MediatR;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.Commands;

public class AdjustInventoryStockCommand : IRequest<Unit>
{
    public Guid InventoryItemId { get; set; }
    public decimal Quantity { get; set; }  // Positive for addition, negative for reduction
    public StockTransactionType TransactionType { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedBy { get; set; }
}
