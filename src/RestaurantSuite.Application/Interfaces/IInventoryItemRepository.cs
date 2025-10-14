using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Interfaces;

public interface IInventoryItemRepository
{
    Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventoryItem?> GetBySKUAsync(string sku, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItem>> GetActiveItemsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default);
    void Update(InventoryItem item);
    void Delete(InventoryItem item);
}
