namespace RestaurantSuite.Domain.Entities;

public class RecipeIngredient
{
    public Guid Id { get; private set; }
    public Guid RecipeId { get; private set; }
    public Guid InventoryItemId { get; private set; }
    public decimal QuantityRequired { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public Recipe Recipe { get; private set; }
    public InventoryItem InventoryItem { get; private set; }

    private RecipeIngredient() { }

    public static RecipeIngredient Create(Guid recipeId, Guid inventoryItemId, decimal quantityRequired)
    {
        if (quantityRequired <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantityRequired));

        return new RecipeIngredient
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            InventoryItemId = inventoryItemId,
            QuantityRequired = quantityRequired,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateQuantity(decimal quantityRequired)
    {
        if (quantityRequired <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantityRequired));

        QuantityRequired = quantityRequired;
    }
}
