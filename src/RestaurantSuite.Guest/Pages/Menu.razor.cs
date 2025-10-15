using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RestaurantSuite.Guest.Models;
using RestaurantSuite.Guest.Services;

namespace RestaurantSuite.Guest.Pages
{
    public partial class Menu : IAsyncDisposable
    {
        [Inject]
        private MenuApiService MenuApiService { get; set; } = default!;

        [Inject]
        private CategoriesApiService CategoriesApiService { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        [Inject]
        private MobileInteractionService MobileService { get; set; } = default!;

        // Component State
        private List<MenuItemDto>? menuItems;
        private List<CategoryDto>? menuCategories;
        private List<MenuItemDto>? filteredMenuItems;
        private string? selectedCategory;
        private bool isLoading = true;

        // Lifecycle Methods
        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine("🎯 Menu.razor OnInitializedAsync() - STARTING");
            try
            {
                await LoadCategories();
                await LoadMenuItems();
                Console.WriteLine("✅ Menu.razor OnInitializedAsync() - COMPLETED SUCCESSFULLY");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Menu.razor OnInitializedAsync() - ERROR: {ex.Message}");
                Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Initialize pull-to-refresh
                MobileService.OnPullToRefreshRequested += HandleRefresh;
                await MobileService.InitializePullToRefreshAsync("menu-page-container");
                Console.WriteLine("✅ Pull-to-refresh initialized on Menu page");
            }
        }

        private async Task HandleRefresh()
        {
            Console.WriteLine("🔄 Refresh triggered by pull-to-refresh");
            await MobileService.HapticAsync("light");

            try
            {
                await LoadCategories();
                await LoadMenuItems();
                StateHasChanged();
                Console.WriteLine("✅ Menu refreshed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error refreshing menu: {ex.Message}");
                await MobileService.HapticAsync("error");
                throw;
            }
        }

        // Data Loading Methods
        private async Task LoadCategories()
        {
            Console.WriteLine("📋 Menu.razor LoadCategories() - Starting");
            try
            {
                Console.WriteLine("📡 Calling CategoriesApiService.GetCategoriesAsync()...");
                var categories = await CategoriesApiService.GetCategoriesAsync();
                Console.WriteLine($"📊 Received {categories?.Count ?? 0} categories from API");
                
                if (categories != null && categories.Any())
                {
                    menuCategories = categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ToList();
                    Console.WriteLine($"✅ Loaded {menuCategories.Count} active categories");
                }
                else
                {
                    Console.WriteLine("⚠️ No categories received from API, using mock data");
                    menuCategories = GetMockCategories();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading categories: {ex.Message}");
                menuCategories = GetMockCategories();
                Console.WriteLine("✅ Fallback to mock categories completed");
            }
        }

        private async Task LoadMenuItems()
        {
            try
            {
                isLoading = true;
                Console.WriteLine("🔄 LoadMenuItems() - Calling MenuApiService.GetMenuItemsAsync()...");
                menuItems = await MenuApiService.GetMenuItemsAsync();
                Console.WriteLine($"✅ LoadMenuItems() - Successfully loaded {menuItems?.Count ?? 0} menu items from API");
                FilterMenuItems();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌❌❌ ERROR LOADING MENU ITEMS FROM API ❌❌❌");
                Console.WriteLine($"❌ Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"❌ Error Message: {ex.Message}");
                Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"❌ Inner Exception: {ex.InnerException.Message}");
                }
                Console.WriteLine("⚠️⚠️⚠️ FALLING BACK TO MOCK DATA ⚠️⚠️⚠️");
                menuItems = await GetMockMenuItems();
                FilterMenuItems();
            }
            finally
            {
                isLoading = false;
            }
        }

        // Event Handlers
        private void SetCategory(string? category)
        {
            selectedCategory = category;
            FilterMenuItems();
        }

        private void FilterMenuItems()
        {
            if (menuItems == null)
            {
                filteredMenuItems = new List<MenuItemDto>();
                return;
            }

            filteredMenuItems = string.IsNullOrEmpty(selectedCategory) 
                ? menuItems 
                : menuItems.Where(item => item.CategoryName == selectedCategory).ToList();
        }

        private async Task AddToOrder(MenuItemDto item)
        {
            await MobileService.HapticAsync("success");
            Console.WriteLine($"Added {item.Name} to order");
            // TODO: Implement actual order functionality
        }

        // Helper Methods
        private string GetCategoryButtonClasses(string? category)
        {
            var baseClasses = "px-4 py-2 rounded-lg font-medium transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2";
            var isSelected = selectedCategory == category;
            
            return isSelected 
                ? $"{baseClasses} bg-primary-600 text-white focus:ring-primary-600" 
                : $"{baseClasses} bg-white text-neutral-700 hover:bg-neutral-100 focus:ring-neutral-500 border border-neutral-300";
        }

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

        private async Task<List<MenuItemDto>> GetMockMenuItems()
        {
            return new List<MenuItemDto>
            {
                new MenuItemDto 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Pljeskavica", 
                    Description = "Traditional Serbian grilled meat patty served with kajmak and ajvar", 
                    Price = 450m, 
                    CategoryName = "Main Course", 
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&h=200&fit=crop",
                    CategoryId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new MenuItemDto 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Ćevapi", 
                    Description = "Grilled minced meat fingers served with somun, onions, and ajvar", 
                    Price = 380m, 
                    CategoryName = "Main Course", 
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400&h=200&fit=crop",
                    CategoryId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new MenuItemDto 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Karađorđeva Šnicla", 
                    Description = "Breaded veal cutlet stuffed with kajmak, served with tartar sauce", 
                    Price = 650m, 
                    CategoryName = "Main Course", 
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=400&h=200&fit=crop",
                    CategoryId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new MenuItemDto 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Palačinke", 
                    Description = "Traditional Serbian pancakes with various fillings", 
                    Price = 280m, 
                    CategoryName = "Dessert", 
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=400&h=200&fit=crop",
                    CategoryId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new MenuItemDto 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Šopska Salata", 
                    Description = "Fresh tomato, cucumber, and pepper salad with sirene cheese", 
                    Price = 220m, 
                    CategoryName = "Appetizer", 
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1571115764595-644a1f56a55c?w=400&h=200&fit=crop",
                    CategoryId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new MenuItemDto 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Rakija", 
                    Description = "Traditional Serbian fruit brandy, various flavors available", 
                    Price = 180m, 
                    CategoryName = "Beverage", 
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1587668178277-295251f900ce?w=400&h=200&fit=crop",
                    CategoryId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new MenuItemDto 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Sarma", 
                    Description = "Cabbage rolls stuffed with minced meat and rice", 
                    Price = 350m, 
                    CategoryName = "Main Course", 
                    IsAvailable = true,
                    ImageUrl = "",
                    CategoryId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new MenuItemDto 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Prebranac", 
                    Description = "Baked beans with onions and paprika", 
                    Price = 200m, 
                    CategoryName = "Appetizer", 
                    IsAvailable = true,
                    ImageUrl = "",
                    CategoryId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new MenuItemDto 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Kajmak", 
                    Description = "Creamy dairy spread, perfect with bread", 
                    Price = 150m, 
                    CategoryName = "Appetizer", 
                    IsAvailable = true,
                    ImageUrl = "",
                    CategoryId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                MobileService.OnPullToRefreshRequested -= HandleRefresh;
                await MobileService.DisposePullToRefreshAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disposing Menu component: {ex.Message}");
            }
        }
    }
}
