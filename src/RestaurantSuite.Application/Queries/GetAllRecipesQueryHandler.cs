using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

public class GetAllRecipesQueryHandler : IRequestHandler<GetAllRecipesQuery, List<RecipeDto>>
{
    private readonly IRecipeRepository _recipeRepository;

    public GetAllRecipesQueryHandler(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<List<RecipeDto>> Handle(GetAllRecipesQuery request, CancellationToken cancellationToken)
    {
        var recipes = await _recipeRepository.GetAllWithIngredientsAsync(cancellationToken);

        // Filter by active status if needed
        if (!request.IncludeInactive)
        {
            recipes = recipes.Where(r => r.IsActive);
        }

        return recipes.Select(recipe => new RecipeDto
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
        }).ToList();
    }
}
