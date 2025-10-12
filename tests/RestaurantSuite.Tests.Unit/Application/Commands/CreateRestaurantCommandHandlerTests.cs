using FluentAssertions;
using Moq;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Commands;

public class CreateRestaurantCommandHandlerTests
{
    private readonly Mock<RestaurantSuite.Application.Interfaces.IRestaurantRepository> _restaurantRepositoryMock;
    private readonly Mock<RestaurantSuite.Application.Interfaces.IUnitOfWork> _unitOfWorkMock;

    public CreateRestaurantCommandHandlerTests()
    {
        _restaurantRepositoryMock = new Mock<RestaurantSuite.Application.Interfaces.IRestaurantRepository>();
        _unitOfWorkMock = new Mock<RestaurantSuite.Application.Interfaces.IUnitOfWork>();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateRestaurant()
    {
        // Arrange
        var command = new RestaurantSuite.Application.Commands.CreateRestaurantCommand
        {
            Name = "Test Restaurant",
            Address = "123 Main St",
            Timezone = "UTC",
            Currency = "USD"
        };

        var handler = new RestaurantSuite.Application.Commands.CreateRestaurantCommandHandler(
            _restaurantRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _restaurantRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RestaurantSuite.Domain.Entities.Restaurant>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidCommand_ShouldThrowException()
    {
        // Arrange
        var command = new RestaurantSuite.Application.Commands.CreateRestaurantCommand
        {
            Name = "",
            Address = "123 Main St",
            Timezone = "UTC",
            Currency = "USD"
        };

        var handler = new RestaurantSuite.Application.Commands.CreateRestaurantCommandHandler(
            _restaurantRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
}
