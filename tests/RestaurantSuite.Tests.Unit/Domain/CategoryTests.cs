using FluentAssertions;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Domain;

public class CategoryTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateCategory()
    {
        // Arrange
        var name = "Appetizers";
        var displayOrder = 1;

        // Act
        var category = RestaurantSuite.Domain.Entities.Category.Create(name, displayOrder);

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(name);
        category.DisplayOrder.Should().Be(displayOrder);
        category.IsActive.Should().BeTrue();
        category.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrowException(string invalidName)
    {
        // Act
        var act = () => RestaurantSuite.Domain.Entities.Category.Create(invalidName, 1);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateDisplayOrder_WithValidOrder_ShouldUpdateDisplayOrder()
    {
        // Arrange
        var category = RestaurantSuite.Domain.Entities.Category.Create("Appetizers", 1);
        var newOrder = 5;

        // Act
        category.UpdateDisplayOrder(newOrder);

        // Assert
        category.DisplayOrder.Should().Be(newOrder);
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldSetIsActiveFalse()
    {
        // Arrange
        var category = RestaurantSuite.Domain.Entities.Category.Create("Appetizers", 1);

        // Act
        category.Deactivate();

        // Assert
        category.IsActive.Should().BeFalse();
    }
}
