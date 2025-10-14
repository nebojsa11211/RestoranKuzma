namespace RestaurantSuite.Chef.Models;

public class MenuItemWithIngredients
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int? PreparationTimeMinutes { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<Ingredient> Ingredients { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class Ingredient
{
    public Guid Id { get; set; }
    public Guid MenuItemId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal QuantityInGrams { get; set; }
    public bool IsMainIngredient { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MenuItemsResponse
{
    public bool Success { get; set; }
    public List<MenuItemWithIngredients> MenuItems { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class CategoriesResponse
{
    public bool Success { get; set; }
    public List<Category> Categories { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
