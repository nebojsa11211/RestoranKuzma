using FluentAssertions;
using Moq;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Commands;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<RestaurantSuite.Application.Interfaces.IOrderRepository> _orderRepositoryMock;
    private readonly Mock<RestaurantSuite.Application.Interfaces.IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<RestaurantSuite.Application.Interfaces.INotificationService> _notificationServiceMock;

    public CreateOrderCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<RestaurantSuite.Application.Interfaces.IOrderRepository>();
        _unitOfWorkMock = new Mock<RestaurantSuite.Application.Interfaces.IUnitOfWork>();
        _notificationServiceMock = new Mock<RestaurantSuite.Application.Interfaces.INotificationService>();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateOrder()
    {
        // Arrange
        var command = new RestaurantSuite.Application.Commands.CreateOrderCommand
        {
            TableId = Guid.NewGuid(),
            WaiterId = Guid.NewGuid(),
            Items = new List<RestaurantSuite.Application.Commands.OrderItemDto>
            {
                new RestaurantSuite.Application.Commands.OrderItemDto
                {
                    MenuItemId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 10.00m,
                    SpecialInstructions = "No onions"
                }
            }
        };

        var handler = new RestaurantSuite.Application.Commands.CreateOrderCommandHandler(
            _orderRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _notificationServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RestaurantSuite.Domain.Entities.Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _notificationServiceMock.Verify(x => x.NotifyNewOrderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyItems_ShouldThrowException()
    {
        // Arrange
        var command = new RestaurantSuite.Application.Commands.CreateOrderCommand
        {
            TableId = Guid.NewGuid(),
            WaiterId = Guid.NewGuid(),
            Items = new List<RestaurantSuite.Application.Commands.OrderItemDto>()
        };

        var handler = new RestaurantSuite.Application.Commands.CreateOrderCommandHandler(
            _orderRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _notificationServiceMock.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
}
