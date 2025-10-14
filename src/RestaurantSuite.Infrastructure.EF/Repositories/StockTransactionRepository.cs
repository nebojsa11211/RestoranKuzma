using Microsoft.EntityFrameworkCore;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Infrastructure.EF.Repositories;

public class StockTransactionRepository : IStockTransactionRepository
{
    private readonly ApplicationDbContext _context;

    public StockTransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StockTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StockTransactions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<StockTransaction>> GetByInventoryItemIdAsync(Guid inventoryItemId, int limit = 100, CancellationToken cancellationToken = default)
    {
        return await _context.StockTransactions
            .Where(t => t.InventoryItemId == inventoryItemId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .Include(t => t.CreatedByUser)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockTransaction>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.StockTransactions
            .Where(t => t.OrderId == orderId)
            .Include(t => t.InventoryItem)
            .Include(t => t.CreatedByUser)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.StockTransactions
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .Include(t => t.InventoryItem)
            .Include(t => t.CreatedByUser)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(StockTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.StockTransactions.AddAsync(transaction, cancellationToken);
    }
}
