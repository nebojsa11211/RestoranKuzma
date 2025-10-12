namespace RestaurantSuite.Application.DTOs;

/// <summary>
/// Request DTO for creating a new menu item
/// </summary>
public class CreateMenuItemDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? ImageUrl { get; set; }
}
