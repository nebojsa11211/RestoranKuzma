using FluentValidation;
using RestaurantSuite.Application.Commands;

namespace RestaurantSuite.Application.Validators;

/// <summary>
/// Validator for UpdateMenuItemPriceCommand
/// </summary>
public class UpdateMenuItemPriceCommandValidator : AbstractValidator<UpdateMenuItemPriceCommand>
{
    public UpdateMenuItemPriceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");
    }
}
