namespace RestaurantSuite.Application.DTOs;

/// <summary>
/// Data transfer object for menu item ingredient information
/// </summary>
public class MenuItemIngredientDto
{
    public Guid Id { get; set; }
    public Guid MenuItemId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal QuantityInGrams { get; set; }
    public bool IsMainIngredient { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
