using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.DTOs;

/// <summary>
/// Data transfer object for creating an order item customization
/// </summary>
public class CreateOrderItemCustomizationDto
{
    public string IngredientName { get; set; } = string.Empty;
    public CustomizationType CustomizationType { get; set; }
    public string? Notes { get; set; }
}
