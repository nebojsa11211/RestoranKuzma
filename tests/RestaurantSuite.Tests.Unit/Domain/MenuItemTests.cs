using FluentAssertions;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Domain;

public class MenuItemTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateMenuItem()
    {
        // Arrange
        var name = "Burger";
        var description = "Delicious burger";
        var price = 9.99m;
        var categoryId = Guid.NewGuid();

        // Act
        var item = RestaurantSuite.Domain.Entities.MenuItem.Create(name, description, price, categoryId);

        // Assert
        item.Should().NotBeNull();
        item.Name.Should().Be(name);
        item.Description.Should().Be(description);
        item.Price.Should().Be(price);
        item.CategoryId.Should().Be(categoryId);
        item.IsAvailable.Should().BeTrue();
        item.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.50)]
    public void Create_WithInvalidPrice_ShouldThrowException(decimal invalidPrice)
    {
        // Act
        var act = () => RestaurantSuite.Domain.Entities.MenuItem.Create(
            "Item", "Desc", invalidPrice, Guid.NewGuid());

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MarkAsUnavailable_WhenAvailable_ShouldSetIsAvailableFalse()
    {
        // Arrange
        var item = RestaurantSuite.Domain.Entities.MenuItem.Create(
            "Item", "Desc", 5.00m, Guid.NewGuid());

        // Act
        item.MarkAsUnavailable();

        // Assert
        item.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void MarkAsAvailable_WhenUnavailable_ShouldSetIsAvailableTrue()
    {
        // Arrange
        var item = RestaurantSuite.Domain.Entities.MenuItem.Create(
            "Item", "Desc", 5.00m, Guid.NewGuid());
        item.MarkAsUnavailable();

        // Act
        item.MarkAsAvailable();

        // Assert
        item.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void UpdatePrice_WithValidPrice_ShouldUpdatePrice()
    {
        // Arrange
        var item = RestaurantSuite.Domain.Entities.MenuItem.Create(
            "Item", "Desc", 5.00m, Guid.NewGuid());
        var newPrice = 7.50m;

        // Act
        item.UpdatePrice(newPrice);

        // Assert
        item.Price.Should().Be(newPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdatePrice_WithInvalidPrice_ShouldThrowException(decimal invalidPrice)
    {
        // Arrange
        var item = RestaurantSuite.Domain.Entities.MenuItem.Create(
            "Item", "Desc", 5.00m, Guid.NewGuid());

        // Act
        var act = () => item.UpdatePrice(invalidPrice);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
