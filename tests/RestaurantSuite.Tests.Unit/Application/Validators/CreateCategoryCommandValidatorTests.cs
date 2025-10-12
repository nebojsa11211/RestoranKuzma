using FluentValidation.TestHelper;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.Validators;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Validators;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator;

    public CreateCategoryCommandValidatorTests()
    {
        _validator = new CreateCategoryCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateCategoryCommand
        {
            Name = "Beverages",
            DisplayOrder = 1
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
        var command = new CreateCategoryCommand
        {
            Name = invalidName!,
            DisplayOrder = 1
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
        var command = new CreateCategoryCommand
        {
            Name = "Be",
            DisplayOrder = 1
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
        var command = new CreateCategoryCommand
        {
            Name = new string('a', 101),
            DisplayOrder = 1
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNegativeDisplayOrder_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateCategoryCommand
        {
            Name = "Beverages",
            DisplayOrder = -1
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
    }
}
