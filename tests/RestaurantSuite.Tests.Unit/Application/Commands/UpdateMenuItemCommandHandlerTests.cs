using FluentAssertions;
using Moq;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Commands;

public class UpdateMenuItemCommandHandlerTests
{
    private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateMenuItemCommandHandlerTests()
    {
        _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUpdateMenuItem()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Beverages", 1);
        var menuItem = MenuItem.Create("Espresso", "Strong coffee", 2.50m, categoryId);

        _menuItemRepositoryMock.Setup(x => x.GetByIdAsync(menuItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(menuItem);

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var command = new UpdateMenuItemCommand
        {
            Id = menuItem.Id,
            Name = "Double Espresso",
            Description = "Very strong coffee",
            CategoryId = categoryId
        };

        var handler = new UpdateMenuItemCommandHandler(
            _menuItemRepositoryMock.Object,
            _categoryRepositoryMock.Object,
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

        var command = new UpdateMenuItemCommand
        {
            Id = Guid.NewGuid(),
            Name = "Double Espresso",
            Description = "Very strong coffee",
            CategoryId = Guid.NewGuid()
        };

        var handler = new UpdateMenuItemCommandHandler(
            _menuItemRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithNonExistentCategory_ShouldThrowException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var menuItem = MenuItem.Create("Espresso", "Strong coffee", 2.50m, categoryId);

        _menuItemRepositoryMock.Setup(x => x.GetByIdAsync(menuItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(menuItem);

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var command = new UpdateMenuItemCommand
        {
            Id = menuItem.Id,
            Name = "Double Espresso",
            Description = "Very strong coffee",
            CategoryId = Guid.NewGuid()
        };

        var handler = new UpdateMenuItemCommandHandler(
            _menuItemRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*not found*");
    }
}
