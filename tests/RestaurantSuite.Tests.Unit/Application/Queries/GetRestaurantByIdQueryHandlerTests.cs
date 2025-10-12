using FluentAssertions;
using Moq;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Queries;

public class GetRestaurantByIdQueryHandlerTests
{
    private readonly Mock<RestaurantSuite.Application.Interfaces.IRestaurantRepository> _restaurantRepositoryMock;

    public GetRestaurantByIdQueryHandlerTests()
    {
        _restaurantRepositoryMock = new Mock<RestaurantSuite.Application.Interfaces.IRestaurantRepository>();
    }

    [Fact]
    public async Task Handle_WithExistingId_ShouldReturnRestaurant()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var restaurant = RestaurantSuite.Domain.Entities.Restaurant.Create("Test", "Address", "UTC", "USD");

        _restaurantRepositoryMock
            .Setup(x => x.GetByIdAsync(restaurantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(restaurant);

        var query = new RestaurantSuite.Application.Queries.GetRestaurantByIdQuery { Id = restaurantId };
        var handler = new RestaurantSuite.Application.Queries.GetRestaurantByIdQueryHandler(_restaurantRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test");
        _restaurantRepositoryMock.Verify(x => x.GetByIdAsync(restaurantId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();

        _restaurantRepositoryMock
            .Setup(x => x.GetByIdAsync(restaurantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RestaurantSuite.Domain.Entities.Restaurant)null);

        var query = new RestaurantSuite.Application.Queries.GetRestaurantByIdQuery { Id = restaurantId };
        var handler = new RestaurantSuite.Application.Queries.GetRestaurantByIdQueryHandler(_restaurantRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
