using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Interfaces;

public interface IRestaurantRepository
{
    Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Restaurant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Restaurant>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Restaurant restaurant, CancellationToken cancellationToken = default);
    void Update(Restaurant restaurant);
    void Delete(Restaurant restaurant);
}
