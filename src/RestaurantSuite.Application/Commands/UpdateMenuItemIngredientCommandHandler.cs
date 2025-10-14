using MediatR;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Handler for updating a menu item ingredient
/// </summary>
public class UpdateMenuItemIngredientCommandHandler : IRequestHandler<UpdateMenuItemIngredientCommand, Unit>
{
    private readonly IMenuItemIngredientRepository _ingredientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMenuItemIngredientCommandHandler(
        IMenuItemIngredientRepository ingredientRepository,
        IUnitOfWork unitOfWork)
    {
        _ingredientRepository = ingredientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateMenuItemIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ingredient == null)
            throw new KeyNotFoundException($"Ingredient with ID {request.Id} not found");

        ingredient.Update(
            request.IngredientName,
            request.QuantityInGrams,
            request.IsMainIngredient,
            request.DisplayOrder);

        _ingredientRepository.Update(ingredient);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
