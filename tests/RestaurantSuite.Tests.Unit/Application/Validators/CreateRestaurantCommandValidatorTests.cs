using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Validators;

public class CreateRestaurantCommandValidatorTests
{
    private readonly RestaurantSuite.Application.Validators.CreateRestaurantCommandValidator _validator;

    public CreateRestaurantCommandValidatorTests()
    {
        _validator = new RestaurantSuite.Application.Validators.CreateRestaurantCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new RestaurantSuite.Application.Commands.CreateRestaurantCommand
        {
            Name = "Test Restaurant",
            Address = "123 Main St",
            Timezone = "UTC",
            Currency = "USD"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WithInvalidName_ShouldHaveValidationError(string invalidName)
    {
        // Arrange
        var command = new RestaurantSuite.Application.Commands.CreateRestaurantCommand
        {
            Name = invalidName,
            Address = "123 Main St",
            Timezone = "UTC",
            Currency = "USD"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithInvalidAddress_ShouldHaveValidationError(string invalidAddress)
    {
        // Arrange
        var command = new RestaurantSuite.Application.Commands.CreateRestaurantCommand
        {
            Name = "Test Restaurant",
            Address = invalidAddress,
            Timezone = "UTC",
            Currency = "USD"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithInvalidTimezone_ShouldHaveValidationError(string invalidTimezone)
    {
        // Arrange
        var command = new RestaurantSuite.Application.Commands.CreateRestaurantCommand
        {
            Name = "Test Restaurant",
            Address = "123 Main St",
            Timezone = invalidTimezone,
            Currency = "USD"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Timezone);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithInvalidCurrency_ShouldHaveValidationError(string invalidCurrency)
    {
        // Arrange
        var command = new RestaurantSuite.Application.Commands.CreateRestaurantCommand
        {
            Name = "Test Restaurant",
            Address = "123 Main St",
            Timezone = "UTC",
            Currency = invalidCurrency
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }
}
