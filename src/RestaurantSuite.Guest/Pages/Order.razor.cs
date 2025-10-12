using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RestaurantSuite.Guest.Models;
using RestaurantSuite.Guest.Services;
using System.Net.Http.Json;

namespace RestaurantSuite.Guest.Pages
{
    public partial class Order
    {
        [Inject]
        private ApiService ApiService { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        // Component State
        private OrderType? selectedOrderType;
        private string? selectedCategory;
        private List<MenuItemDto> menuItems = new();
        private List<CategoryDto> menuCategories = new();
        private List<CartItem> cartItems = new();
        private bool isSubmitting = false;
        private bool showSuccessModal = false;

        // Lifecycle Methods
        protected override async Task OnInitializedAsync()
        {
            await LoadCategories();
            await LoadMenuItems();
        }

        // Data Loading Methods
        private async Task LoadCategories()
        {
            try
            {
                var categories = await ApiService.GetCategoriesAsync();
                menuCategories = categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading categories: {ex.Message}");
                menuCategories = GetMockCategories();
            }
        }

        private async Task LoadMenuItems()
        {
            try
            {
                menuItems = await ApiService.GetMenuItemsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading menu items: {ex.Message}");
                menuItems = GetMockMenuItems();
            }
        }

        // Event Handler Methods
        private void HandleDineInClick()
        {
            selectedOrderType = OrderType.DineIn;
        }

        private void HandleTakeawayClick()
        {
            selectedOrderType = OrderType.Takeaway;
        }

        private void HandleCategoryAllClick()
        {
            selectedCategory = null;
        }

        private void HandleCategoryClick(string categoryName)
        {
            selectedCategory = categoryName;
        }

        private void HandleAddToCart(Guid itemId)
        {
            var item = menuItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null && item.IsAvailable)
            {
                var existingItem = cartItems.FirstOrDefault(ci => ci.Item.Id == itemId);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    cartItems.Add(new CartItem { Item = item, Quantity = 1 });
                }
            }
        }

        private void HandleDecreaseQuantity(Guid itemId)
        {
            var cartItem = cartItems.FirstOrDefault(ci => ci.Item.Id == itemId);
            if (cartItem != null)
            {
                cartItem.Quantity--;
                if (cartItem.Quantity <= 0)
                {
                    cartItems.Remove(cartItem);
                }
            }
        }

        private void HandleIncreaseQuantity(Guid itemId)
        {
            var cartItem = cartItems.FirstOrDefault(ci => ci.Item.Id == itemId);
            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
        }

        private void HandleRemoveFromCart(Guid itemId)
        {
            var cartItem = cartItems.FirstOrDefault(ci => ci.Item.Id == itemId);
            if (cartItem != null)
            {
                cartItems.Remove(cartItem);
            }
        }

        private async Task HandleCheckout()
        {
            if (!selectedOrderType.HasValue || !cartItems.Any())
            {
                return;
            }

            try
            {
                isSubmitting = true;
                await Task.Delay(2000);
                Console.WriteLine($"Order placed: {selectedOrderType.Value}, Total: ${GetCartTotal()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error placing order: {ex.Message}");
            }
            finally
            {
                isSubmitting = false;
            }
        }

        // Helper Methods
        private List<CategoryDto> GetMockCategories()
        {
            return new List<CategoryDto>
            {
                new CategoryDto { Id = Guid.NewGuid(), Name = "Appetizers", Description = "Start your meal with our delicious appetizers", DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CategoryDto { Id = Guid.NewGuid(), Name = "Main Courses", Description = "Hearty main dishes from Serbian cuisine", DisplayOrder = 2, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CategoryDto { Id = Guid.NewGuid(), Name = "Desserts", Description = "Sweet endings to your meal", DisplayOrder = 3, IsActive = true, CreatedAt = DateTime.UtcNow },
                new CategoryDto { Id = Guid.NewGuid(), Name = "Beverages", Description = "Traditional drinks and beverages", DisplayOrder = 4, IsActive = true, CreatedAt = DateTime.UtcNow }
            };
        }

        private List<MenuItemDto> GetMockMenuItems()
        {
            return new List<MenuItemDto>
            {
                new MenuItemDto { Id = Guid.NewGuid(), Name = "Pljeskavica", Description = "Traditional Serbian grilled meat patty served with kajmak and ajvar", Price = 450m, CategoryName = "Appetizers", IsAvailable = true, CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                new MenuItemDto { Id = Guid.NewGuid(), Name = "Ćevapi", Description = "Grilled minced meat fingers served with somun, onions, and ajvar", Price = 380m, CategoryName = "Main Course", IsAvailable = true, CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                new MenuItemDto { Id = Guid.NewGuid(), Name = "Karađorđeva Šnicla", Description = "Breaded veal cutlet stuffed with kajmak, served with tartar sauce", Price = 650m, CategoryName = "Main Course", IsAvailable = true, CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                new MenuItemDto { Id = Guid.NewGuid(), Name = "Palačinke", Description = "Traditional Serbian pancakes with various fillings", Price = 280m, CategoryName = "Dessert", IsAvailable = true, CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                new MenuItemDto { Id = Guid.NewGuid(), Name = "Šopska Salata", Description = "Fresh tomato, cucumber, and pepper salad with sirene cheese", Price = 220m, CategoryName = "Appetizer", IsAvailable = true, CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                new MenuItemDto { Id = Guid.NewGuid(), Name = "Rakija", Description = "Traditional Serbian fruit brandy, various flavors available", Price = 180m, CategoryName = "Beverage", IsAvailable = true, CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                new MenuItemDto { Id = Guid.NewGuid(), Name = "Sarma", Description = "Cabbage rolls stuffed with minced meat and rice", Price = 350m, CategoryName = "Main Course", IsAvailable = true, CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                new MenuItemDto { Id = Guid.NewGuid(), Name = "Prebranac", Description = "Baked beans with onions and paprika", Price = 200m, CategoryName = "Appetizer", IsAvailable = true, CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                new MenuItemDto { Id = Guid.NewGuid(), Name = "Kajmak", Description = "Creamy dairy spread, perfect with bread", Price = 150m, CategoryName = "Appetizer", IsAvailable = true, CategoryId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow }
            };
        }

        private decimal GetCartTotal()
        {
            return cartItems.Sum(ci => ci.Item.Price * ci.Quantity);
        }

        // Properties
        private List<MenuItemDto> filteredMenuItems => 
            selectedCategory == null 
                ? menuItems 
                : menuItems.Where(item => item.CategoryName == selectedCategory).ToList();

        // Helper Classes
        private class CartItem
        {
            public MenuItemDto Item { get; set; } = new();
            public int Quantity { get; set; } = 1;
        }

        private enum OrderType
        {
            DineIn,
            Takeaway
        }
    }
}
