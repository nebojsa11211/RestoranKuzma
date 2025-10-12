namespace RestaurantSuite.Application.DTOs;

/// <summary>
/// Request DTO for updating menu item details
/// </summary>
public class UpdateMenuItemDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? ImageUrl { get; set; }
}
