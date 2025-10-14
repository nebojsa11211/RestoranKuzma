using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Interfaces;

/// <summary>
/// Repository interface for MenuItemIngredient entity
/// </summary>
public interface IMenuItemIngredientRepository
{
    Task<MenuItemIngredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuItemIngredient>> GetByMenuItemIdAsync(Guid menuItemId, CancellationToken cancellationToken = default);
    Task AddAsync(MenuItemIngredient ingredient, CancellationToken cancellationToken = default);
    void Update(MenuItemIngredient ingredient);
    void Delete(MenuItemIngredient ingredient);
}
