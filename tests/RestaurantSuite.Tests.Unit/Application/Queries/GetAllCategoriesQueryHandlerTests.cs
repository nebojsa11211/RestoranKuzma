using FluentAssertions;
using Moq;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Application.Queries;
using RestaurantSuite.Domain.Entities;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Queries;

public class GetAllCategoriesQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;

    public GetAllCategoriesQueryHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            Category.Create("Beverages", 1),
            Category.Create("Food", 2)
        };

        _categoryRepositoryMock.Setup(x => x.GetByRestaurantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var query = new GetAllCategoriesQuery();
        var handler = new GetAllCategoriesQueryHandler(_categoryRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Beverages");
        result[1].Name.Should().Be("Food");
    }

    [Fact]
    public async Task Handle_WithNoCategories_ShouldReturnEmptyList()
    {
        // Arrange
        _categoryRepositoryMock.Setup(x => x.GetByRestaurantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var query = new GetAllCategoriesQuery();
        var handler = new GetAllCategoriesQueryHandler(_categoryRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
