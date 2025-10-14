using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to update a menu item ingredient
/// </summary>
public class UpdateMenuItemIngredientCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal QuantityInGrams { get; set; }
    public bool IsMainIngredient { get; set; }
    public int DisplayOrder { get; set; }
}
