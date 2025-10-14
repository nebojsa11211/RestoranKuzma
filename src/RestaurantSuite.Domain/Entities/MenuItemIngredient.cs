namespace RestaurantSuite.Domain.Entities;

public class MenuItemIngredient
{
    public Guid Id { get; private set; }
    public Guid MenuItemId { get; private set; }
    public string IngredientName { get; private set; }
    public decimal QuantityInGrams { get; private set; }
    public bool IsMainIngredient { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public MenuItem MenuItem { get; private set; }

    private MenuItemIngredient() { }

    public static MenuItemIngredient Create(
        Guid menuItemId,
        string ingredientName,
        decimal quantityInGrams,
        bool isMainIngredient,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(ingredientName))
            throw new ArgumentException("Ingredient name cannot be empty", nameof(ingredientName));

        if (quantityInGrams <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantityInGrams));

        return new MenuItemIngredient
        {
            Id = Guid.NewGuid(),
            MenuItemId = menuItemId,
            IngredientName = ingredientName.Trim(),
            QuantityInGrams = quantityInGrams,
            IsMainIngredient = isMainIngredient,
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string ingredientName,
        decimal quantityInGrams,
        bool isMainIngredient,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(ingredientName))
            throw new ArgumentException("Ingredient name cannot be empty", nameof(ingredientName));

        if (quantityInGrams <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantityInGrams));

        IngredientName = ingredientName.Trim();
        QuantityInGrams = quantityInGrams;
        IsMainIngredient = isMainIngredient;
        DisplayOrder = displayOrder;
    }
}
