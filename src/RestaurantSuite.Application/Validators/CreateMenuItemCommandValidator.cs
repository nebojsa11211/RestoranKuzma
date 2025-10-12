using FluentValidation;
using RestaurantSuite.Application.Commands;

namespace RestaurantSuite.Application.Validators;

/// <summary>
/// Validator for CreateMenuItemCommand
/// </summary>
public class CreateMenuItemCommandValidator : AbstractValidator<CreateMenuItemCommand>
{
    public CreateMenuItemCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("ImageUrl must not exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));
    }
}
