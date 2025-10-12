using Microsoft.EntityFrameworkCore;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Infrastructure.EF.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly ApplicationDbContext _context;

    public RestaurantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Restaurants.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Restaurant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Restaurants.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Restaurant>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        // Restaurant entity no longer has IsActive property - it's a singleton configuration entity
        // Return all restaurants (should be only one in single-restaurant architecture)
        return await _context.Restaurants
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
    {
        await _context.Restaurants.AddAsync(restaurant, cancellationToken);
    }

    public void Update(Restaurant restaurant)
    {
        _context.Restaurants.Update(restaurant);
    }

    public void Delete(Restaurant restaurant)
    {
        _context.Restaurants.Remove(restaurant);
    }
}
