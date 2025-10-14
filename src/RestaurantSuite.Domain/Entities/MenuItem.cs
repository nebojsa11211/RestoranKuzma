namespace RestaurantSuite.Domain.Entities;

public class MenuItem
{
    public Guid Id { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsAvailable { get; private set; }
    public int? PreparationTimeMinutes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; } = null;

    // Navigation properties
    public Category Category { get; private set; }

    private readonly List<MenuItemIngredient> _ingredients = new();
    public IReadOnlyCollection<MenuItemIngredient> Ingredients => _ingredients.AsReadOnly();

    private MenuItem() { }

    public static MenuItem Create(string name, string description, decimal price, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));

        return new MenuItem
        {
            Id = Guid.NewGuid(),
            CategoryId = categoryId,
            Name = name,
            Description = description,
            Price = price,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(newPrice));

        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string description, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetImageUrl(string imageUrl)
    {
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsAvailable()
    {
        IsAvailable = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUnavailable()
    {
        IsAvailable = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPreparationTime(int? preparationTimeMinutes)
    {
        if (preparationTimeMinutes.HasValue && preparationTimeMinutes.Value <= 0)
            throw new ArgumentException("Preparation time must be greater than zero", nameof(preparationTimeMinutes));

        PreparationTimeMinutes = preparationTimeMinutes;
        UpdatedAt = DateTime.UtcNow;
    }

    public Guid AddIngredient(string ingredientName, decimal quantityInGrams, bool isMainIngredient, int displayOrder)
    {
        var ingredient = MenuItemIngredient.Create(Id, ingredientName, quantityInGrams, isMainIngredient, displayOrder);
        _ingredients.Add(ingredient);
        UpdatedAt = DateTime.UtcNow;
        return ingredient.Id;
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
}
