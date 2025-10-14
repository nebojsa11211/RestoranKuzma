namespace RestaurantSuite.Application.DTOs;

/// <summary>
/// Data transfer object for recipe information
/// </summary>
public class RecipeDto
{
    public Guid Id { get; set; }
    public Guid MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<RecipeIngredientDto> Ingredients { get; set; } = new();
}
