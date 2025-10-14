using Microsoft.EntityFrameworkCore;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Infrastructure.EF.Repositories;

public class MenuItemRepository : IMenuItemRepository
{
    private readonly ApplicationDbContext _context;

    public MenuItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MenuItems.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<MenuItem?> GetByIdWithIngredientsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MenuItems
            .Include(m => m.Ingredients.OrderBy(i => i.DisplayOrder))
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<MenuItem>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        // Single-restaurant architecture: no RestaurantId filtering needed
        return await _context.MenuItems
            .Include(m => m.Category)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MenuItem>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.MenuItems
            .Where(m => m.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MenuItem>> GetAvailableByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        // Single-restaurant architecture: no RestaurantId filtering needed
        return await _context.MenuItems
            .Where(m => m.IsAvailable)
            .Include(m => m.Category)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MenuItem>> GetAvailableWithIngredientsAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        // Single-restaurant architecture: no RestaurantId filtering needed
        return await _context.MenuItems
            .Where(m => m.IsAvailable)
            .Include(m => m.Category)
            .Include(m => m.Ingredients.OrderBy(i => i.DisplayOrder))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
    {
        await _context.MenuItems.AddAsync(menuItem, cancellationToken);
    }

    public void Update(MenuItem menuItem)
    {
        _context.MenuItems.Update(menuItem);
    }

    public void Delete(MenuItem menuItem)
    {
        _context.MenuItems.Remove(menuItem);
    }
}
