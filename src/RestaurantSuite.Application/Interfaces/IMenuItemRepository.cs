using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Interfaces;

public interface IMenuItemRepository
{
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MenuItem?> GetByIdWithIngredientsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuItem>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuItem>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuItem>> GetAvailableByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuItem>> GetAvailableWithIngredientsAsync(Guid restaurantId, CancellationToken cancellationToken = default);
    Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken = default);
    void Update(MenuItem menuItem);
    void Delete(MenuItem menuItem);
}
