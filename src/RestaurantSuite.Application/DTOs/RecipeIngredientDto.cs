using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.DTOs;

/// <summary>
/// Data transfer object for recipe ingredient information
/// </summary>
public class RecipeIngredientDto
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public Guid InventoryItemId { get; set; }
    public string InventoryItemName { get; set; } = string.Empty;
    public string InventoryItemSKU { get; set; } = string.Empty;
    public decimal QuantityRequired { get; set; }
    public UnitOfMeasure Unit { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
