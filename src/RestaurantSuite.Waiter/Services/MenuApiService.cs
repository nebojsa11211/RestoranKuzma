using System.Net.Http.Json;
using System.Net.Http.Headers;
using RestaurantSuite.Waiter.Services;

namespace RestaurantSuite.Waiter.Services;

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

    public async Task<List<MenuItemDto>> GetMenuItemsAsync(Guid? categoryId = null)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var queryString = categoryId.HasValue ? $"?categoryId={categoryId}" : "";
            var response = await _httpClient.GetAsync($"api/menu{queryString}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<MenuItemDto>>() ?? new List<MenuItemDto>();
            }

            Console.WriteLine($"Failed to fetch menu items: {response.ReasonPhrase}");
            return new List<MenuItemDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetMenuItemsAsync: {ex.Message}");
            return new List<MenuItemDto>();
        }
    }
}

// Additional DTOs used by MenuApiService
public class MenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string? ImageUrl { get; set; }
}
