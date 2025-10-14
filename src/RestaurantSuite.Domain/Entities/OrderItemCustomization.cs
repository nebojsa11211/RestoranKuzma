using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Domain.Entities;

public class OrderItemCustomization
{
    public Guid Id { get; private set; }
    public Guid OrderItemId { get; private set; }
    public string IngredientName { get; private set; }
    public CustomizationType CustomizationType { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public OrderItem OrderItem { get; private set; }

    private OrderItemCustomization() { }

    public static OrderItemCustomization Create(
        Guid orderItemId,
        string ingredientName,
        CustomizationType customizationType,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(ingredientName))
            throw new ArgumentException("Ingredient name cannot be empty", nameof(ingredientName));

        if (!string.IsNullOrEmpty(notes) && notes.Length > 200)
            throw new ArgumentException("Notes cannot exceed 200 characters", nameof(notes));

        return new OrderItemCustomization
        {
            Id = Guid.NewGuid(),
            OrderItemId = orderItemId,
            IngredientName = ingredientName.Trim(),
            CustomizationType = customizationType,
            Notes = notes?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string ingredientName,
        CustomizationType customizationType,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(ingredientName))
            throw new ArgumentException("Ingredient name cannot be empty", nameof(ingredientName));

        if (!string.IsNullOrEmpty(notes) && notes.Length > 200)
            throw new ArgumentException("Notes cannot exceed 200 characters", nameof(notes));

        IngredientName = ingredientName.Trim();
        CustomizationType = customizationType;
        Notes = notes?.Trim();
    }
}
