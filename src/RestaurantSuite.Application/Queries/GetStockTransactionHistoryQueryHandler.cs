using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

public class GetStockTransactionHistoryQueryHandler : IRequestHandler<GetStockTransactionHistoryQuery, List<StockTransactionDto>>
{
    private readonly IStockTransactionRepository _stockTransactionRepository;

    public GetStockTransactionHistoryQueryHandler(IStockTransactionRepository stockTransactionRepository)
    {
        _stockTransactionRepository = stockTransactionRepository;
    }

    public async Task<List<StockTransactionDto>> Handle(GetStockTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.StockTransaction> transactions;

        // Get transactions based on filters
        if (request.InventoryItemId.HasValue)
        {
            transactions = await _stockTransactionRepository.GetByInventoryItemIdAsync(
                request.InventoryItemId.Value,
                request.Limit,
                cancellationToken);
        }
        else if (request.OrderId.HasValue)
        {
            transactions = await _stockTransactionRepository.GetByOrderIdAsync(request.OrderId.Value, cancellationToken);
        }
        else if (request.FromDate.HasValue && request.ToDate.HasValue)
        {
            // Use date range filter
            transactions = await _stockTransactionRepository.GetByDateRangeAsync(
                request.FromDate.Value,
                request.ToDate.Value,
                cancellationToken);
        }
        else
        {
            // No specific filter - use date range for recent transactions (last 30 days)
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-30);
            transactions = await _stockTransactionRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        }

        // Apply additional date range filter if only one date is specified
        if (request.FromDate.HasValue && !request.ToDate.HasValue)
        {
            transactions = transactions.Where(t => t.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue && !request.FromDate.HasValue)
        {
            transactions = transactions.Where(t => t.CreatedAt <= request.ToDate.Value);
        }

        // Apply limit and map to DTOs
        return transactions
            .OrderByDescending(t => t.CreatedAt)
            .Take(request.Limit)
            .Select(t => new StockTransactionDto
            {
                Id = t.Id,
                InventoryItemId = t.InventoryItemId,
                InventoryItemName = t.InventoryItem?.Name ?? "Unknown",
                TransactionType = t.TransactionType,
                Quantity = t.Quantity,
                PreviousStock = t.PreviousStock,
                NewStock = t.NewStock,
                OrderId = t.OrderId,
                Notes = t.Notes,
                CreatedBy = t.CreatedBy,
                CreatedAt = t.CreatedAt
            })
            .ToList();
    }
}
