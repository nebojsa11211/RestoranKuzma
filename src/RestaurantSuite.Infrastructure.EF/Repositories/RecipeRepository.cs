using Microsoft.EntityFrameworkCore;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Infrastructure.EF.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly ApplicationDbContext _context;

    public RecipeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Recipes.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Recipe?> GetByIdWithIngredientsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Recipes
            .Include(r => r.Ingredients)
                .ThenInclude(i => i.InventoryItem)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Recipe?> GetByMenuItemIdAsync(Guid menuItemId, CancellationToken cancellationToken = default)
    {
        return await _context.Recipes
            .FirstOrDefaultAsync(r => r.MenuItemId == menuItemId, cancellationToken);
    }

    public async Task<Recipe?> GetByMenuItemIdWithIngredientsAsync(Guid menuItemId, CancellationToken cancellationToken = default)
    {
        return await _context.Recipes
            .Include(r => r.Ingredients)
                .ThenInclude(i => i.InventoryItem)
            .Include(r => r.MenuItem)
            .FirstOrDefaultAsync(r => r.MenuItemId == menuItemId, cancellationToken);
    }

    public async Task<IEnumerable<Recipe>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Recipes
            .Include(r => r.MenuItem)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Recipe>> GetAllWithIngredientsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Recipes
            .Include(r => r.MenuItem)
            .Include(r => r.Ingredients)
                .ThenInclude(i => i.InventoryItem)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        await _context.Recipes.AddAsync(recipe, cancellationToken);
    }

    public void Update(Recipe recipe)
    {
        _context.Recipes.Update(recipe);
    }

    public void Delete(Recipe recipe)
    {
        _context.Recipes.Remove(recipe);
    }
}
