using FluentAssertions;
using Moq;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Commands;

public class UpdateMenuItemPriceCommandHandlerTests
{
    private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateMenuItemPriceCommandHandlerTests()
    {
        _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUpdatePrice()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var menuItem = MenuItem.Create("Espresso", "Strong coffee", 2.50m, categoryId);

        _menuItemRepositoryMock.Setup(x => x.GetByIdAsync(menuItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(menuItem);

        var command = new UpdateMenuItemPriceCommand
        {
            Id = menuItem.Id,
            Price = 3.00m
        };

        var handler = new UpdateMenuItemPriceCommandHandler(
            _menuItemRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _menuItemRepositoryMock.Verify(x => x.Update(It.IsAny<MenuItem>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentMenuItem_ShouldThrowException()
    {
        // Arrange
        _menuItemRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MenuItem?)null);

        var command = new UpdateMenuItemPriceCommand
        {
            Id = Guid.NewGuid(),
            Price = 3.00m
        };

        var handler = new UpdateMenuItemPriceCommandHandler(
            _menuItemRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
