using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to retrieve stock transaction history for an inventory item
/// </summary>
public class GetStockTransactionHistoryQuery : IRequest<List<StockTransactionDto>>
{
    /// <summary>
    /// Filter by inventory item (optional)
    /// </summary>
    public Guid? InventoryItemId { get; set; }

    /// <summary>
    /// Filter by order (optional)
    /// </summary>
    public Guid? OrderId { get; set; }

    /// <summary>
    /// Filter by date range - start date (optional)
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Filter by date range - end date (optional)
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Maximum number of results to return (default: 100)
    /// </summary>
    public int Limit { get; set; } = 100;
}
