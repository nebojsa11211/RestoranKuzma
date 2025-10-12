using FluentAssertions;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Domain;

public class UserTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var email = "test@example.com";
        var firstName = "John";
        var lastName = "Doe";
        var role = RestaurantSuite.Domain.Enums.UserRole.Waiter;

        // Act
        var user = RestaurantSuite.Domain.Entities.User.Create(email, firstName, lastName, role);

        // Assert
        user.Should().NotBeNull();
        user.Email.Should().Be(email);
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.Role.Should().Be(role);
        user.IsActive.Should().BeTrue();
        user.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("invalid-email")]
    public void Create_WithInvalidEmail_ShouldThrowException(string invalidEmail)
    {
        // Act
        var act = () => RestaurantSuite.Domain.Entities.User.Create(
            invalidEmail, "John", "Doe", RestaurantSuite.Domain.Enums.UserRole.Waiter);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldSetIsActiveFalse()
    {
        // Arrange
        var user = RestaurantSuite.Domain.Entities.User.Create(
            "test@example.com", "John", "Doe", RestaurantSuite.Domain.Enums.UserRole.Waiter);

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_WhenInactive_ShouldSetIsActiveTrue()
    {
        // Arrange
        var user = RestaurantSuite.Domain.Entities.User.Create(
            "test@example.com", "John", "Doe", RestaurantSuite.Domain.Enums.UserRole.Waiter);
        user.Deactivate();

        // Act
        user.Activate();

        // Assert
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldUpdateUser()
    {
        // Arrange
        var user = RestaurantSuite.Domain.Entities.User.Create(
            "test@example.com", "John", "Doe", RestaurantSuite.Domain.Enums.UserRole.Waiter);
        var newFirstName = "Jane";
        var newLastName = "Smith";
        var newPhone = "+1234567890";

        // Act
        user.UpdateProfile(newFirstName, newLastName, newPhone);

        // Assert
        user.FirstName.Should().Be(newFirstName);
        user.LastName.Should().Be(newLastName);
        user.Phone.Should().Be(newPhone);
    }
}
