using MediatR;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Commands;

public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Guid>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRecipeCommandHandler(
        IRecipeRepository recipeRepository,
        IMenuItemRepository menuItemRepository,
        IInventoryItemRepository inventoryItemRepository,
        IUnitOfWork unitOfWork)
    {
        _recipeRepository = recipeRepository;
        _menuItemRepository = menuItemRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        // Verify menu item exists
        var menuItem = await _menuItemRepository.GetByIdAsync(request.MenuItemId, cancellationToken);
        if (menuItem == null)
            throw new InvalidOperationException($"Menu item {request.MenuItemId} not found");

        // Check if recipe already exists for this menu item
        var existingRecipe = await _recipeRepository.GetByMenuItemIdAsync(request.MenuItemId, cancellationToken);
        if (existingRecipe != null)
            throw new InvalidOperationException($"A recipe already exists for menu item '{menuItem.Name}'");

        // Create recipe
        var recipe = Recipe.Create(request.MenuItemId);

        // Add ingredients
        foreach (var ingredientInput in request.Ingredients)
        {
            // Verify inventory item exists
            var inventoryItem = await _inventoryItemRepository.GetByIdAsync(ingredientInput.InventoryItemId, cancellationToken);
            if (inventoryItem == null)
                throw new InvalidOperationException($"Inventory item {ingredientInput.InventoryItemId} not found");

            recipe.AddIngredient(ingredientInput.InventoryItemId, ingredientInput.QuantityRequired);
        }

        await _recipeRepository.AddAsync(recipe, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return recipe.Id;
    }
}
