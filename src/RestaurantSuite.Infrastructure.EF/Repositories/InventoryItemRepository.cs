using Microsoft.EntityFrameworkCore;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Infrastructure.EF.Repositories;

public class InventoryItemRepository : IInventoryItemRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<InventoryItem?> GetBySKUAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.SKU == sku.ToUpperInvariant(), cancellationToken);
    }

    public async Task<IEnumerable<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryItem>> GetActiveItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .Where(i => i.IsActive)
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .Where(i => i.IsActive && i.CurrentStock <= i.MinimumStockLevel)
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        await _context.InventoryItems.AddAsync(item, cancellationToken);
    }

    public void Update(InventoryItem item)
    {
        _context.InventoryItems.Update(item);
    }

    public void Delete(InventoryItem item)
    {
        _context.InventoryItems.Remove(item);
    }
}
