using FluentAssertions;
using Moq;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Queries;

public class GetOrdersByRestaurantQueryHandlerTests
{
    private readonly Mock<RestaurantSuite.Application.Interfaces.IOrderRepository> _orderRepositoryMock;

    public GetOrdersByRestaurantQueryHandlerTests()
    {
        _orderRepositoryMock = new Mock<RestaurantSuite.Application.Interfaces.IOrderRepository>();
    }

    [Fact]
    public async Task Handle_WithExistingOrders_ShouldReturnOrders()
    {
        // Arrange
        var orders = new List<RestaurantSuite.Domain.Entities.Order>
        {
            RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), Guid.NewGuid()),
            RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), Guid.NewGuid())
        };

        _orderRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        var query = new RestaurantSuite.Application.Queries.GetOrdersByRestaurantQuery();
        var handler = new RestaurantSuite.Application.Queries.GetOrdersByRestaurantQueryHandler(_orderRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoOrders_ShouldReturnEmptyList()
    {
        // Arrange
        _orderRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RestaurantSuite.Domain.Entities.Order>());

        var query = new RestaurantSuite.Application.Queries.GetOrdersByRestaurantQuery();
        var handler = new RestaurantSuite.Application.Queries.GetOrdersByRestaurantQueryHandler(_orderRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
