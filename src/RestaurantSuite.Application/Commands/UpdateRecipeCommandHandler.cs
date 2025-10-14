using MediatR;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Commands;

public class UpdateRecipeCommandHandler : IRequestHandler<UpdateRecipeCommand, Unit>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRecipeCommandHandler(
        IRecipeRepository recipeRepository,
        IInventoryItemRepository inventoryItemRepository,
        IUnitOfWork unitOfWork)
    {
        _recipeRepository = recipeRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        // Get existing recipe
        var recipe = await _recipeRepository.GetByMenuItemIdWithIngredientsAsync(request.MenuItemId, cancellationToken);
        if (recipe == null)
            throw new InvalidOperationException($"No recipe found for menu item {request.MenuItemId}");

        // Clear existing ingredients
        recipe.ClearIngredients();

        // Add new ingredients
        foreach (var ingredientInput in request.Ingredients)
        {
            // Verify inventory item exists
            var inventoryItem = await _inventoryItemRepository.GetByIdAsync(ingredientInput.InventoryItemId, cancellationToken);
            if (inventoryItem == null)
                throw new InvalidOperationException($"Inventory item {ingredientInput.InventoryItemId} not found");

            recipe.AddIngredient(ingredientInput.InventoryItemId, ingredientInput.QuantityRequired);
        }

        _recipeRepository.Update(recipe);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
