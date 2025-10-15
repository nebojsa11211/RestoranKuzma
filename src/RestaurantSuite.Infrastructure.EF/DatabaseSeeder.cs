using Microsoft.EntityFrameworkCore;
using RestaurantSuite.Domain.Entities;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Infrastructure.EF;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;

    public DatabaseSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if we already have data
        if (await _context.Categories.AnyAsync() || await _context.MenuItems.AnyAsync())
        {
            Console.WriteLine("Database already seeded, skipping...");
            return;
        }

        Console.WriteLine("Starting database seeding...");

        // 1. Create Pizza Category
        var pizzaCategory = Category.Create("Pizza", 1);

        await _context.Categories.AddAsync(pizzaCategory);
        await _context.SaveChangesAsync();
        Console.WriteLine($"✓ Pizza category created");

        // 2. Create Inventory Items (Pizza Ingredients)
        var inventoryItems = new List<InventoryItem>
        {
            InventoryItem.Create("Pizza Dough", "Fresh pizza dough", "ING-001", 50, UnitOfMeasure.Kilograms, 10, 2.50m),
            InventoryItem.Create("Tomato Sauce", "Italian tomato sauce", "ING-002", 30, UnitOfMeasure.Liters, 5, 3.00m),
            InventoryItem.Create("Mozzarella Cheese", "Fresh mozzarella", "ING-003", 40, UnitOfMeasure.Kilograms, 8, 8.00m),
            InventoryItem.Create("Pepperoni", "Sliced pepperoni", "ING-004", 20, UnitOfMeasure.Kilograms, 5, 12.00m),
            InventoryItem.Create("Italian Sausage", "Spicy Italian sausage", "ING-005", 15, UnitOfMeasure.Kilograms, 5, 10.00m),
            InventoryItem.Create("Mushrooms", "Fresh mushrooms", "ING-006", 10, UnitOfMeasure.Kilograms, 3, 5.00m),
            InventoryItem.Create("Bell Peppers", "Mixed bell peppers", "ING-007", 12, UnitOfMeasure.Kilograms, 3, 4.00m),
            InventoryItem.Create("Onions", "Red onions", "ING-008", 15, UnitOfMeasure.Kilograms, 4, 2.00m),
            InventoryItem.Create("Black Olives", "Sliced black olives", "ING-009", 8, UnitOfMeasure.Kilograms, 2, 6.00m),
            InventoryItem.Create("Fresh Basil", "Fresh basil leaves", "ING-010", 5, UnitOfMeasure.Kilograms, 1, 15.00m),
            InventoryItem.Create("Parmesan Cheese", "Grated parmesan", "ING-011", 10, UnitOfMeasure.Kilograms, 2, 18.00m),
            InventoryItem.Create("Ham", "Sliced ham", "ING-012", 12, UnitOfMeasure.Kilograms, 3, 9.00m),
            InventoryItem.Create("Pineapple", "Canned pineapple chunks", "ING-013", 20, UnitOfMeasure.Kilograms, 5, 3.50m),
            InventoryItem.Create("Bacon", "Crispy bacon", "ING-014", 10, UnitOfMeasure.Kilograms, 3, 11.00m),
            InventoryItem.Create("Spinach", "Fresh spinach", "ING-015", 8, UnitOfMeasure.Kilograms, 2, 4.50m),
            InventoryItem.Create("Ricotta Cheese", "Creamy ricotta", "ING-016", 10, UnitOfMeasure.Kilograms, 2, 7.00m),
            InventoryItem.Create("Garlic", "Fresh garlic cloves", "ING-017", 5, UnitOfMeasure.Kilograms, 1, 8.00m),
            InventoryItem.Create("Jalapeños", "Sliced jalapeños", "ING-018", 6, UnitOfMeasure.Kilograms, 2, 5.50m),
            InventoryItem.Create("BBQ Sauce", "Smoky BBQ sauce", "ING-019", 10, UnitOfMeasure.Liters, 3, 4.00m),
            InventoryItem.Create("Chicken Breast", "Grilled chicken breast", "ING-020", 15, UnitOfMeasure.Kilograms, 5, 13.00m),
            InventoryItem.Create("Feta Cheese", "Crumbled feta", "ING-021", 8, UnitOfMeasure.Kilograms, 2, 10.00m),
            InventoryItem.Create("Sun-dried Tomatoes", "Sun-dried tomatoes in oil", "ING-022", 5, UnitOfMeasure.Kilograms, 1, 12.00m),
            InventoryItem.Create("Arugula", "Fresh arugula", "ING-023", 6, UnitOfMeasure.Kilograms, 2, 6.00m),
            InventoryItem.Create("Prosciutto", "Thinly sliced prosciutto", "ING-024", 5, UnitOfMeasure.Kilograms, 1, 22.00m)
        };

        await _context.InventoryItems.AddRangeAsync(inventoryItems);
        await _context.SaveChangesAsync();
        Console.WriteLine($"✓ {inventoryItems.Count} inventory items created");

        // Helper to get inventory item by name
        var getInventoryItem = (string name) => inventoryItems.First(i => i.Name == name);

        // 3. Create 15 Pizza Menu Items with Recipes
        var pizzaData = new List<(string name, string description, decimal price, int prepTime, string image, List<(string ingredient, decimal quantity)> recipe)>
        {
            ("Margherita", "Classic pizza with tomato sauce, mozzarella, and fresh basil", 890, 15,
                "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Tomato Sauce", 0.1m), ("Mozzarella Cheese", 0.15m), ("Fresh Basil", 0.01m) }),

            ("Pepperoni", "Loaded with pepperoni and mozzarella cheese", 1050, 15,
                "https://images.unsplash.com/photo-1628840042765-356cda07504e?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Tomato Sauce", 0.1m), ("Mozzarella Cheese", 0.15m), ("Pepperoni", 0.1m) }),

            ("Quattro Formaggi", "Four cheese blend: mozzarella, parmesan, ricotta, and feta", 1150, 18,
                "https://images.unsplash.com/photo-1571407970349-bc81e7e96b47?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Tomato Sauce", 0.08m), ("Mozzarella Cheese", 0.1m), ("Parmesan Cheese", 0.05m), ("Ricotta Cheese", 0.08m), ("Feta Cheese", 0.05m) }),

            ("Hawaiian", "Ham, pineapple, and mozzarella on tomato base", 1100, 16,
                "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Tomato Sauce", 0.1m), ("Mozzarella Cheese", 0.15m), ("Ham", 0.08m), ("Pineapple", 0.08m) }),

            ("Vegetarian Supreme", "Mushrooms, bell peppers, onions, olives, and fresh vegetables", 1080, 17,
                "https://images.unsplash.com/photo-1511689660979-10d2b1aada49?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Tomato Sauce", 0.1m), ("Mozzarella Cheese", 0.15m), ("Mushrooms", 0.06m), ("Bell Peppers", 0.06m), ("Onions", 0.04m), ("Black Olives", 0.04m) }),

            ("Meat Lovers", "Pepperoni, Italian sausage, ham, and bacon", 1250, 18,
                "https://images.unsplash.com/photo-1534308983496-4fabb1a015ee?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Tomato Sauce", 0.1m), ("Mozzarella Cheese", 0.15m), ("Pepperoni", 0.06m), ("Italian Sausage", 0.06m), ("Ham", 0.06m), ("Bacon", 0.06m) }),

            ("BBQ Chicken", "Grilled chicken with BBQ sauce, onions, and mozzarella", 1180, 20,
                "https://images.unsplash.com/photo-1555072956-7758afb20e8f?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("BBQ Sauce", 0.08m), ("Mozzarella Cheese", 0.15m), ("Chicken Breast", 0.12m), ("Onions", 0.05m) }),

            ("Spicy Italian", "Italian sausage, jalapeños, onions, and spicy tomato sauce", 1120, 16,
                "https://images.unsplash.com/photo-1593504049359-74330189a345?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Tomato Sauce", 0.1m), ("Mozzarella Cheese", 0.15m), ("Italian Sausage", 0.1m), ("Jalapeños", 0.04m), ("Onions", 0.04m) }),

            ("White Pizza", "Garlic oil base with ricotta, mozzarella, and parmesan", 1090, 17,
                "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Garlic", 0.02m), ("Mozzarella Cheese", 0.12m), ("Ricotta Cheese", 0.1m), ("Parmesan Cheese", 0.05m) }),

            ("Mediterranean", "Feta cheese, sun-dried tomatoes, olives, and spinach", 1140, 17,
                "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Tomato Sauce", 0.1m), ("Mozzarella Cheese", 0.12m), ("Feta Cheese", 0.08m), ("Sun-dried Tomatoes", 0.05m), ("Black Olives", 0.05m), ("Spinach", 0.04m) }),

            ("Prosciutto e Rucola", "Prosciutto, arugula, parmesan, and mozzarella", 1320, 18,
                "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Tomato Sauce", 0.08m), ("Mozzarella Cheese", 0.12m), ("Prosciutto", 0.08m), ("Arugula", 0.04m), ("Parmesan Cheese", 0.04m) }),

            ("Mushroom Truffle", "Mixed mushrooms, mozzarella, parmesan, and garlic", 1280, 19,
                "https://images.unsplash.com/photo-1571997478779-2adcbbe9ab2f?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Tomato Sauce", 0.08m), ("Mozzarella Cheese", 0.15m), ("Mushrooms", 0.12m), ("Garlic", 0.02m), ("Parmesan Cheese", 0.05m) }),

            ("Spinach & Ricotta", "Creamy ricotta, fresh spinach, garlic, and mozzarella", 1060, 16,
                "https://images.unsplash.com/photo-1595854341625-f33ee10dbf94?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Tomato Sauce", 0.08m), ("Mozzarella Cheese", 0.12m), ("Ricotta Cheese", 0.1m), ("Spinach", 0.08m), ("Garlic", 0.02m) }),

            ("Supreme Deluxe", "Everything pizza: pepperoni, sausage, mushrooms, peppers, onions, olives", 1290, 20,
                "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Tomato Sauce", 0.1m), ("Mozzarella Cheese", 0.15m), ("Pepperoni", 0.06m), ("Italian Sausage", 0.06m), ("Mushrooms", 0.05m), ("Bell Peppers", 0.05m), ("Onions", 0.04m), ("Black Olives", 0.04m) }),

            ("Carbonara Pizza", "Bacon, eggs, parmesan, and black pepper on creamy base", 1190, 18,
                "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=400&h=300&fit=crop",
                new List<(string, decimal)> { ("Pizza Dough", 0.3m), ("Mozzarella Cheese", 0.15m), ("Bacon", 0.1m), ("Parmesan Cheese", 0.08m), ("Garlic", 0.02m) })
        };

        // Create menu items and recipes
        var recipeCount = 0;
        foreach (var (name, description, price, prepTime, image, recipeIngredients) in pizzaData)
        {
            // Create menu item
            var menuItem = MenuItem.Create(name, description, price, pizzaCategory.Id);
            menuItem.SetImageUrl(image);
            menuItem.SetPreparationTime(prepTime);

            await _context.MenuItems.AddAsync(menuItem);
            await _context.SaveChangesAsync();

            // Create recipe for this menu item
            var recipe = Recipe.Create(menuItem.Id);
            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();

            // Add ingredients to the recipe using domain method
            foreach (var (ingredientName, quantity) in recipeIngredients)
            {
                var inventoryItem = getInventoryItem(ingredientName);
                recipe.AddIngredient(inventoryItem.Id, quantity);
            }

            await _context.SaveChangesAsync();
            recipeCount++;
        }

        Console.WriteLine($"✓ {pizzaData.Count} pizza menu items created");
        Console.WriteLine($"✓ {recipeCount} recipes created with ingredients");

        Console.WriteLine("✅ Database seeding completed successfully!");
        Console.WriteLine($"   - 1 Pizza category");
        Console.WriteLine($"   - {inventoryItems.Count} inventory items");
        Console.WriteLine($"   - {pizzaData.Count} pizza menu items");
        Console.WriteLine($"   - {recipeCount} recipes with ingredients");
    }
}
