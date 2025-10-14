using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to add an ingredient to a menu item
/// </summary>
public class AddMenuItemIngredientCommand : IRequest<Guid>
{
    public Guid MenuItemId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal QuantityInGrams { get; set; }
    public bool IsMainIngredient { get; set; }
    public int DisplayOrder { get; set; }
}
