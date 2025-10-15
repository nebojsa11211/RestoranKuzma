using System.Net.Http.Json;
using System.Net.Http.Headers;
using RestaurantSuite.Chef.Models;

namespace RestaurantSuite.Chef.Services;

public class MenuApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public MenuApiService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    private async Task EnsureAuthenticatedAsync()
    {
        var token = await _authService.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<MenuItemsResponse> GetMenuItemsWithRecipesAsync(Guid restaurantId, Guid? categoryId = null)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var queryString = $"?restaurantId={restaurantId}";
            if (categoryId.HasValue)
            {
                queryString += $"&categoryId={categoryId.Value}";
            }

            var response = await _httpClient.GetAsync($"api/chef/menu-with-recipes{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var menuItems = await response.Content.ReadFromJsonAsync<List<MenuItemWithIngredients>>();
                return new MenuItemsResponse
                {
                    Success = true,
                    MenuItems = menuItems ?? new List<MenuItemWithIngredients>()
                };
            }

            return new MenuItemsResponse
            {
                Success = false,
                ErrorMessage = $"Failed to fetch menu items: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetMenuItemsWithRecipesAsync: {ex.Message}");
            return new MenuItemsResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
