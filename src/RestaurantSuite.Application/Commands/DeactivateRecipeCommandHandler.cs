using MediatR;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Commands;

public class DeactivateRecipeCommandHandler : IRequestHandler<DeactivateRecipeCommand, Unit>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateRecipeCommandHandler(
        IRecipeRepository recipeRepository,
        IUnitOfWork unitOfWork)
    {
        _recipeRepository = recipeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeactivateRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetByMenuItemIdAsync(request.MenuItemId, cancellationToken);
        if (recipe == null)
            throw new InvalidOperationException($"No recipe found for menu item {request.MenuItemId}");

        recipe.Deactivate();

        _recipeRepository.Update(recipe);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
