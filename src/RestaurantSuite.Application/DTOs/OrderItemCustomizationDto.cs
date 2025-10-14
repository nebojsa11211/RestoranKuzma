using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.DTOs;

/// <summary>
/// Data transfer object for order item customization information
/// </summary>
public class OrderItemCustomizationDto
{
    public Guid Id { get; set; }
    public Guid OrderItemId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public CustomizationType CustomizationType { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
