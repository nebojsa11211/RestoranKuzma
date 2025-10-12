using FluentAssertions;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Domain;

public class RestaurantTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateRestaurant()
    {
        // Arrange
        var name = "Test Restaurant";
        var address = "123 Main St";
        var timezone = "UTC";
        var currency = "USD";

        // Act
        var restaurant = RestaurantSuite.Domain.Entities.Restaurant.Create(name, address, timezone, currency);

        // Assert
        restaurant.Should().NotBeNull();
        restaurant.Name.Should().Be(name);
        restaurant.Address.Should().Be(address);
        restaurant.Timezone.Should().Be(timezone);
        restaurant.Currency.Should().Be(currency);
        restaurant.SettingsJson.Should().Be("{}");
        restaurant.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrowException(string invalidName)
    {
        // Act
        var act = () => RestaurantSuite.Domain.Entities.Restaurant.Create(invalidName, "Address", "UTC", "USD");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateDetails_WithValidData_ShouldUpdateRestaurant()
    {
        // Arrange
        var restaurant = RestaurantSuite.Domain.Entities.Restaurant.Create("Old", "Old Address", "UTC", "USD");
        var newName = "New Name";
        var newAddress = "New Address";
        var newTimezone = "America/New_York";
        var newCurrency = "EUR";

        // Act
        restaurant.UpdateDetails(newName, newAddress, newTimezone, newCurrency);

        // Assert
        restaurant.Name.Should().Be(newName);
        restaurant.Address.Should().Be(newAddress);
        restaurant.Timezone.Should().Be(newTimezone);
        restaurant.Currency.Should().Be(newCurrency);
    }

    [Fact]
    public void UpdateSettings_WithValidJson_ShouldUpdateSettings()
    {
        // Arrange
        var restaurant = RestaurantSuite.Domain.Entities.Restaurant.Create("Test", "Address", "UTC", "USD");
        var newSettings = "{\"theme\":\"dark\"}";

        // Act
        restaurant.UpdateSettings(newSettings);

        // Assert
        restaurant.SettingsJson.Should().Be(newSettings);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateSettings_WithInvalidJson_ShouldThrowException(string invalidJson)
    {
        // Arrange
        var restaurant = RestaurantSuite.Domain.Entities.Restaurant.Create("Test", "Address", "UTC", "USD");

        // Act
        var act = () => restaurant.UpdateSettings(invalidJson);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
