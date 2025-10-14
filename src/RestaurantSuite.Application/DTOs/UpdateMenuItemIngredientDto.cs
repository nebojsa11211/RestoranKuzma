namespace RestaurantSuite.Application.DTOs;

/// <summary>
/// Data transfer object for updating a menu item ingredient
/// </summary>
public class UpdateMenuItemIngredientDto
{
    public string IngredientName { get; set; } = string.Empty;
    public decimal QuantityInGrams { get; set; }
    public bool IsMainIngredient { get; set; }
    public int DisplayOrder { get; set; }
}
