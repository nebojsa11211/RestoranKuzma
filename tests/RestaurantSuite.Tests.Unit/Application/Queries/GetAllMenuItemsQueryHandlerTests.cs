using FluentAssertions;
using Moq;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Application.Queries;
using RestaurantSuite.Domain.Entities;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Queries;

public class GetAllMenuItemsQueryHandlerTests
{
    private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock;

    public GetAllMenuItemsQueryHandlerTests()
    {
        _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
    }

    [Fact]
    public async Task Handle_WithNoFilters_ShouldReturnAllMenuItems()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Beverages", 1);
        var menuItems = new List<MenuItem>
        {
            MenuItem.Create("Espresso", "Strong coffee", 2.50m, categoryId),
            MenuItem.Create("Latte", "Milk coffee", 3.50m, categoryId)
        };

        _menuItemRepositoryMock.Setup(x => x.GetByRestaurantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(menuItems);

        var query = new GetAllMenuItemsQuery();
        var handler = new GetAllMenuItemsQueryHandler(_menuItemRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        _menuItemRepositoryMock.Verify(x => x.GetByRestaurantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCategoryFilter_ShouldReturnFilteredMenuItems()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var menuItems = new List<MenuItem>
        {
            MenuItem.Create("Espresso", "Strong coffee", 2.50m, categoryId)
        };

        _menuItemRepositoryMock.Setup(x => x.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(menuItems);

        var query = new GetAllMenuItemsQuery { CategoryId = categoryId };
        var handler = new GetAllMenuItemsQueryHandler(_menuItemRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        _menuItemRepositoryMock.Verify(x => x.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithAvailableOnlyFilter_ShouldReturnOnlyAvailableItems()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var menuItems = new List<MenuItem>
        {
            MenuItem.Create("Espresso", "Strong coffee", 2.50m, categoryId)
        };

        _menuItemRepositoryMock.Setup(x => x.GetAvailableByRestaurantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(menuItems);

        var query = new GetAllMenuItemsQuery { AvailableOnly = true };
        var handler = new GetAllMenuItemsQueryHandler(_menuItemRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        _menuItemRepositoryMock.Verify(x => x.GetAvailableByRestaurantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
