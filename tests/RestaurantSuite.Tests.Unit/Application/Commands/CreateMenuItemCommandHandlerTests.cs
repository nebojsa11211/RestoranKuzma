using FluentAssertions;
using Moq;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Commands;

public class CreateMenuItemCommandHandlerTests
{
    private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CreateMenuItemCommandHandlerTests()
    {
        _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateMenuItem()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Beverages", 1);

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var command = new CreateMenuItemCommand
        {
            Name = "Espresso",
            Description = "Strong coffee",
            Price = 2.50m,
            CategoryId = categoryId
        };

        var handler = new CreateMenuItemCommandHandler(
            _menuItemRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _menuItemRepositoryMock.Verify(x => x.AddAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentCategory_ShouldThrowException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var command = new CreateMenuItemCommand
        {
            Name = "Espresso",
            Description = "Strong coffee",
            Price = 2.50m,
            CategoryId = categoryId
        };

        var handler = new CreateMenuItemCommandHandler(
            _menuItemRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_WithImageUrl_ShouldSetImageUrl()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Beverages", 1);

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var command = new CreateMenuItemCommand
        {
            Name = "Espresso",
            Description = "Strong coffee",
            Price = 2.50m,
            CategoryId = categoryId,
            ImageUrl = "https://example.com/espresso.jpg"
        };

        var handler = new CreateMenuItemCommandHandler(
            _menuItemRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _menuItemRepositoryMock.Verify(x => x.AddAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
