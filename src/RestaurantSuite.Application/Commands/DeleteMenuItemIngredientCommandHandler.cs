using MediatR;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Handler for deleting a menu item ingredient
/// </summary>
public class DeleteMenuItemIngredientCommandHandler : IRequestHandler<DeleteMenuItemIngredientCommand, Unit>
{
    private readonly IMenuItemIngredientRepository _ingredientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMenuItemIngredientCommandHandler(
        IMenuItemIngredientRepository ingredientRepository,
        IUnitOfWork unitOfWork)
    {
        _ingredientRepository = ingredientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteMenuItemIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ingredient == null)
            throw new KeyNotFoundException($"Ingredient with ID {request.Id} not found");

        _ingredientRepository.Delete(ingredient);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
