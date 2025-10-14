using Microsoft.EntityFrameworkCore;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Infrastructure.EF.Repositories;

public class MenuItemIngredientRepository : IMenuItemIngredientRepository
{
    private readonly ApplicationDbContext _context;

    public MenuItemIngredientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MenuItemIngredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MenuItemIngredients.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<MenuItemIngredient>> GetByMenuItemIdAsync(Guid menuItemId, CancellationToken cancellationToken = default)
    {
        return await _context.MenuItemIngredients
            .Where(i => i.MenuItemId == menuItemId)
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MenuItemIngredient ingredient, CancellationToken cancellationToken = default)
    {
        await _context.MenuItemIngredients.AddAsync(ingredient, cancellationToken);
    }

    public void Update(MenuItemIngredient ingredient)
    {
        _context.MenuItemIngredients.Update(ingredient);
    }

    public void Delete(MenuItemIngredient ingredient)
    {
        _context.MenuItemIngredients.Remove(ingredient);
    }
}
