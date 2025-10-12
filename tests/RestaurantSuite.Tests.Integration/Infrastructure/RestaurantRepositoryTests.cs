using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace RestaurantSuite.Tests.Integration.Infrastructure;

public class RestaurantRepositoryTests : IAsyncLifetime
{
    private RestaurantSuite.Infrastructure.EF.ApplicationDbContext _context;
    private RestaurantSuite.Infrastructure.EF.Repositories.RestaurantRepository _repository;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<RestaurantSuite.Infrastructure.EF.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new RestaurantSuite.Infrastructure.EF.ApplicationDbContext(options);
        _repository = new RestaurantSuite.Infrastructure.EF.Repositories.RestaurantRepository(_context);

        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_ShouldAddRestaurantToDatabase()
    {
        // Arrange
        var restaurant = RestaurantSuite.Domain.Entities.Restaurant.Create("Test Restaurant", "123 Main St", "UTC", "USD");

        // Act
        await _repository.AddAsync(restaurant, CancellationToken.None);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Restaurants.FindAsync(restaurant.Id);
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Restaurant");
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnRestaurant()
    {
        // Arrange
        var restaurant = RestaurantSuite.Domain.Entities.Restaurant.Create("Test Restaurant", "123 Main St", "UTC", "USD");
        await _repository.AddAsync(restaurant, CancellationToken.None);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(restaurant.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(restaurant.Id);
        result.Name.Should().Be("Test Restaurant");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Update_ShouldUpdateRestaurantInDatabase()
    {
        // Arrange
        var restaurant = RestaurantSuite.Domain.Entities.Restaurant.Create("Old Name", "Old Address", "UTC", "USD");
        await _repository.AddAsync(restaurant, CancellationToken.None);
        await _context.SaveChangesAsync();

        // Act
        restaurant.UpdateDetails("New Name", "New Address", "UTC", "USD");
        _repository.Update(restaurant);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Restaurants.FindAsync(restaurant.Id);
        result.Should().NotBeNull();
        result.Name.Should().Be("New Name");
        result.Address.Should().Be("New Address");
        result.Timezone.Should().Be("UTC");
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRestaurants()
    {
        // Arrange
        var restaurant1 = RestaurantSuite.Domain.Entities.Restaurant.Create("Restaurant 1", "Address 1", "UTC", "USD");
        var restaurant2 = RestaurantSuite.Domain.Entities.Restaurant.Create("Restaurant 2", "Address 2", "UTC", "EUR");
        await _repository.AddAsync(restaurant1, CancellationToken.None);
        await _repository.AddAsync(restaurant2, CancellationToken.None);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Name == "Restaurant 1");
        result.Should().Contain(r => r.Name == "Restaurant 2");
    }
}
