using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Interfaces;

public interface IStockTransactionRepository
{
    Task<StockTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockTransaction>> GetByInventoryItemIdAsync(Guid inventoryItemId, int limit = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockTransaction>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(StockTransaction transaction, CancellationToken cancellationToken = default);
}
