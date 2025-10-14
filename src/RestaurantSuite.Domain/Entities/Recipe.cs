namespace RestaurantSuite.Domain.Entities;

public class Recipe
{
    public Guid Id { get; private set; }
    public Guid MenuItemId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public MenuItem MenuItem { get; private set; }

    private readonly List<RecipeIngredient> _ingredients = new();
    public IReadOnlyCollection<RecipeIngredient> Ingredients => _ingredients.AsReadOnly();

    private Recipe() { }

    public static Recipe Create(Guid menuItemId)
    {
        return new Recipe
        {
            Id = Guid.NewGuid(),
            MenuItemId = menuItemId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public Guid AddIngredient(Guid inventoryItemId, decimal quantityRequired)
    {
        if (quantityRequired <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantityRequired));

        // Check if ingredient already exists
        if (_ingredients.Any(i => i.InventoryItemId == inventoryItemId))
            throw new InvalidOperationException($"Inventory item {inventoryItemId} is already part of this recipe");

        var ingredient = RecipeIngredient.Create(Id, inventoryItemId, quantityRequired);
        _ingredients.Add(ingredient);
        UpdatedAt = DateTime.UtcNow;
        return ingredient.Id;
    }

    public void UpdateIngredient(Guid ingredientId, decimal quantityRequired)
    {
        var ingredient = _ingredients.FirstOrDefault(i => i.Id == ingredientId);
        if (ingredient == null)
            throw new InvalidOperationException($"Ingredient {ingredientId} not found in recipe");

        ingredient.UpdateQuantity(quantityRequired);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveIngredient(Guid ingredientId)
    {
        var ingredient = _ingredients.FirstOrDefault(i => i.Id == ingredientId);
        if (ingredient != null)
        {
            _ingredients.Remove(ingredient);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ClearIngredients()
    {
        _ingredients.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
