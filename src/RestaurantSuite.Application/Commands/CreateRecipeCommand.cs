using MediatR;

namespace RestaurantSuite.Application.Commands;

public class CreateRecipeCommand : IRequest<Guid>
{
    public Guid MenuItemId { get; set; }
    public List<RecipeIngredientInput> Ingredients { get; set; } = new();
}

public class RecipeIngredientInput
{
    public Guid InventoryItemId { get; set; }
    public decimal QuantityRequired { get; set; }
}
