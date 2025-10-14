using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to update a recipe's ingredients
/// </summary>
public class UpdateRecipeCommand : IRequest<Unit>
{
    public Guid MenuItemId { get; set; }
    public List<RecipeIngredientInput> Ingredients { get; set; } = new();
}
