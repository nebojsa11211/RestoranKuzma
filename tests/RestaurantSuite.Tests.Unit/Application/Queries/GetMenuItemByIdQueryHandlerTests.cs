using FluentAssertions;
using Moq;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Application.Queries;
using RestaurantSuite.Domain.Entities;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Queries;

public class GetMenuItemByIdQueryHandlerTests
{
    private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock;

    public GetMenuItemByIdQueryHandlerTests()
    {
        _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
    }

    [Fact]
    public async Task Handle_WithExistingId_ShouldReturnMenuItem()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var menuItem = MenuItem.Create("Espresso", "Strong coffee", 2.50m, categoryId);

        _menuItemRepositoryMock.Setup(x => x.GetByIdAsync(menuItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(menuItem);

        var query = new GetMenuItemByIdQuery { Id = menuItem.Id };
        var handler = new GetMenuItemByIdQueryHandler(_menuItemRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(menuItem.Id);
        result.Name.Should().Be("Espresso");
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        _menuItemRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MenuItem?)null);

        var query = new GetMenuItemByIdQuery { Id = Guid.NewGuid() };
        var handler = new GetMenuItemByIdQueryHandler(_menuItemRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
