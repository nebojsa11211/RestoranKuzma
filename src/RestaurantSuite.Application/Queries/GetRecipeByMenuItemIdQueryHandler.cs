using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

public class GetRecipeByMenuItemIdQueryHandler : IRequestHandler<GetRecipeByMenuItemIdQuery, RecipeDto?>
{
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipeByMenuItemIdQueryHandler(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<RecipeDto?> Handle(GetRecipeByMenuItemIdQuery request, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetByMenuItemIdWithIngredientsAsync(request.MenuItemId, cancellationToken);

        if (recipe == null)
            return null;

        return new RecipeDto
        {
            Id = recipe.Id,
            MenuItemId = recipe.MenuItemId,
            MenuItemName = recipe.MenuItem?.Name ?? "Unknown",
            IsActive = recipe.IsActive,
            Ingredients = recipe.Ingredients.Select(i => new RecipeIngredientDto
            {
                Id = i.Id,
                RecipeId = i.RecipeId,
                InventoryItemId = i.InventoryItemId,
                InventoryItemName = i.InventoryItem?.Name ?? "Unknown",
                QuantityRequired = i.QuantityRequired,
                Unit = i.InventoryItem?.Unit ?? Domain.Enums.UnitOfMeasure.Pieces
            }).ToList(),
            CreatedAt = recipe.CreatedAt,
            UpdatedAt = recipe.UpdatedAt
        };
    }
}
