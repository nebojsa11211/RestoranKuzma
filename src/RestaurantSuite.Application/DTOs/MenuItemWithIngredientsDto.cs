namespace RestaurantSuite.Application.DTOs;

/// <summary>
/// Data transfer object for menu item with full recipe details (for admin/chef view)
/// </summary>
public class MenuItemWithIngredientsDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int? PreparationTimeMinutes { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<MenuItemIngredientDto> Ingredients { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
