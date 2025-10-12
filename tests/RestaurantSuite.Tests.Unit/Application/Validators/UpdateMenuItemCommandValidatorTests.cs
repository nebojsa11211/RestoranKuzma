using FluentValidation.TestHelper;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.Validators;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Application.Validators;

public class UpdateMenuItemCommandValidatorTests
{
    private readonly UpdateMenuItemCommandValidator _validator;

    public UpdateMenuItemCommandValidatorTests()
    {
        _validator = new UpdateMenuItemCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdateMenuItemCommand
        {
            Id = Guid.NewGuid(),
            Name = "Espresso",
            Description = "Strong coffee",
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
        var command = new UpdateMenuItemCommand
        {
            Id = Guid.NewGuid(),
            Name = invalidName!,
            Description = "Strong coffee",
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithEmptyId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateMenuItemCommand
        {
            Id = Guid.Empty,
            Name = "Espresso",
            Description = "Strong coffee",
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
