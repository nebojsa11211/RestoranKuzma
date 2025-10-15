using System.Net.Http.Json;
using RestaurantSuite.Guest.Models;

namespace RestaurantSuite.Guest.Services;

public class MenuApiService
{
    private readonly HttpClient _httpClient;

    public MenuApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MenuItemDto>> GetMenuItemsAsync(Guid? categoryId = null, bool? availableOnly = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (categoryId.HasValue)
                queryParams.Add($"categoryId={categoryId.Value}");
            if (availableOnly.HasValue)
                queryParams.Add($"availableOnly={availableOnly.Value}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var url = $"api/menu{queryString}";

            Console.WriteLine($"[GetMenuItemsAsync] Requesting: {_httpClient.BaseAddress}{url}");

            var response = await _httpClient.GetAsync(url);

            Console.WriteLine($"[GetMenuItemsAsync] Response Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[GetMenuItemsAsync] API Error: {response.StatusCode} - {response.ReasonPhrase}");
                Console.WriteLine($"[GetMenuItemsAsync] Error Content: {errorContent}");
                throw new HttpRequestException($"API returned {response.StatusCode}: {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadFromJsonAsync<List<MenuItemDto>>();
            var itemCount = content?.Count ?? 0;
            Console.WriteLine($"[GetMenuItemsAsync] Successfully retrieved {itemCount} menu items");
            return content ?? new List<MenuItemDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[GetMenuItemsAsync] HTTP Request Error: {ex.Message}");
            Console.WriteLine($"[GetMenuItemsAsync] Base Address: {_httpClient.BaseAddress}");
            Console.WriteLine($"[GetMenuItemsAsync] This usually means the API server is not running or not accessible.");
            throw;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"[GetMenuItemsAsync] Request Timeout: {ex.Message}");
            Console.WriteLine($"[GetMenuItemsAsync] This usually means the API server is taking too long to respond.");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetMenuItemsAsync] Unexpected Error: {ex.Message}");
            Console.WriteLine($"[GetMenuItemsAsync] Exception Type: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[GetMenuItemsAsync] Inner Exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    public async Task<List<GuestMenuItemDto>> GetGuestMenuAsync(Guid restaurantId, Guid? categoryId = null)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"restaurantId={restaurantId}"
            };

            if (categoryId.HasValue)
                queryParams.Add($"categoryId={categoryId.Value}");

            var queryString = "?" + string.Join("&", queryParams);
            var url = $"api/guest/menu{queryString}";

            Console.WriteLine($"[GetGuestMenuAsync] Requesting: {_httpClient.BaseAddress}{url}");

            var response = await _httpClient.GetAsync(url);

            Console.WriteLine($"[GetGuestMenuAsync] Response Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[GetGuestMenuAsync] API Error: {response.StatusCode} - {response.ReasonPhrase}");
                Console.WriteLine($"[GetGuestMenuAsync] Error Content: {errorContent}");
                throw new HttpRequestException($"API returned {response.StatusCode}: {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadFromJsonAsync<List<GuestMenuItemDto>>();
            var itemCount = content?.Count ?? 0;
            Console.WriteLine($"[GetGuestMenuAsync] Successfully retrieved {itemCount} menu items with ingredients");
            return content ?? new List<GuestMenuItemDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetGuestMenuAsync] Error: {ex.Message}");
            throw;
        }
    }

    public async Task<MenuItemDto?> GetMenuItemByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<MenuItemDto>($"api/menu/{id}");
    }
}
