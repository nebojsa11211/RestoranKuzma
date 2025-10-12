using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace RestaurantSuite.Tests.Integration.Infrastructure;

public class OrderRepositoryTests : IAsyncLifetime
{
    private RestaurantSuite.Infrastructure.EF.ApplicationDbContext _context;
    private RestaurantSuite.Infrastructure.EF.Repositories.OrderRepository _repository;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<RestaurantSuite.Infrastructure.EF.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new RestaurantSuite.Infrastructure.EF.ApplicationDbContext(options);
        _repository = new RestaurantSuite.Infrastructure.EF.Repositories.OrderRepository(_context);

        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_ShouldAddOrderToDatabase()
    {
        // Arrange
        var order = RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), 2, 10.00m, "No onions");

        // Act
        await _repository.AddAsync(order, CancellationToken.None);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == order.Id);
        result.Should().NotBeNull();
        result.OrderItems.Should().HaveCount(1);
        result.TotalAmount.Should().Be(20.00m);
    }


    [Fact]
    public async Task GetByIdWithItemsAsync_ShouldReturnOrderWithItems()
    {
        // Arrange
        var order = RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), 2, 10.00m, "No onions");
        order.AddItem(Guid.NewGuid(), 1, 15.00m, "Extra cheese");

        await _repository.AddAsync(order, CancellationToken.None);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdWithItemsAsync(order.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.OrderItems.Should().HaveCount(2);
        result.TotalAmount.Should().Be(35.00m);
    }

    [Fact]
    public async Task GetActiveOrdersByTableIdAsync_ShouldReturnActiveOrders()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var activeOrder = RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), tableId, Guid.NewGuid());
        var completedOrder = RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), tableId, Guid.NewGuid());
        completedOrder.ConfirmOrder();
        completedOrder.StartPreparation();
        completedOrder.CompletePreparation();
        completedOrder.ServeOrder();
        completedOrder.CompleteOrder();

        await _repository.AddAsync(activeOrder, CancellationToken.None);
        await _repository.AddAsync(completedOrder, CancellationToken.None);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveOrdersByTableIdAsync(tableId, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(activeOrder.Id);
    }
}
