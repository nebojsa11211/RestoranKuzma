using FluentValidation.TestHelper;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.Validators;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Validators;

public class CreateMenuItemCommandValidatorTests
{
    private readonly CreateMenuItemCommandValidator _validator;

    public CreateMenuItemCommandValidatorTests()
    {
        _validator = new CreateMenuItemCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateMenuItemCommand
        {
            Name = "Espresso",
            Description = "Strong coffee",
            Price = 2.50m,
            CategoryId = Guid.NewGuid()
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
    public void Validate_WithInvalidName_ShouldHaveValidationError(string? invalidName)
    {
        // Arrange
        var command = new CreateMenuItemCommand
        {
            Name = invalidName!,
            Description = "Strong coffee",
            Price = 2.50m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNameTooShort_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateMenuItemCommand
        {
            Name = "Es",
            Description = "Strong coffee",
            Price = 2.50m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNameTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateMenuItemCommand
        {
            Name = new string('a', 151),
            Description = "Strong coffee",
            Price = 2.50m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.5)]
    public void Validate_WithInvalidPrice_ShouldHaveValidationError(decimal invalidPrice)
    {
        // Arrange
        var command = new CreateMenuItemCommand
        {
            Name = "Espresso",
            Description = "Strong coffee",
            Price = invalidPrice,
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WithInvalidDescription_ShouldHaveValidationError(string? invalidDescription)
    {
        // Arrange
        var command = new CreateMenuItemCommand
        {
            Name = "Espresso",
            Description = invalidDescription!,
            Price = 2.50m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
