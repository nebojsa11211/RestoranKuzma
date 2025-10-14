using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Interfaces;

public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Recipe?> GetByIdWithIngredientsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Recipe?> GetByMenuItemIdAsync(Guid menuItemId, CancellationToken cancellationToken = default);
    Task<Recipe?> GetByMenuItemIdWithIngredientsAsync(Guid menuItemId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Recipe>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Recipe>> GetAllWithIngredientsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Recipe recipe, CancellationToken cancellationToken = default);
    void Update(Recipe recipe);
    void Delete(Recipe recipe);
}
