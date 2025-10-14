using System.Net.Http.Json;
using RestaurantSuite.Guest.Models;

namespace RestaurantSuite.Guest.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Menu Items
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

    // Categories
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/categories");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
                return content ?? new List<CategoryDto>();
            }
            else
            {
                Console.WriteLine($"API Error in GetCategoriesAsync: {response.StatusCode} - {response.ReasonPhrase}");
                return new List<CategoryDto>();
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Request Error in GetCategoriesAsync: {ex.Message}");
            Console.WriteLine($"Base Address: {_httpClient.BaseAddress}");
            return new List<CategoryDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error in GetCategoriesAsync: {ex.Message}");
            return new List<CategoryDto>();
        }
    }

    // Orders
    public async Task<Guid> CreateOrderAsync(CreateOrderDto order)
    {
        var response = await _httpClient.PostAsJsonAsync("api/orders", order);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        return result?.Id ?? Guid.Empty;
    }

    // Tables
    public async Task<List<TableDto>> GetTablesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TableDto>>("api/tables") ?? new List<TableDto>();
    }

    public async Task<TableDto?> GetTableByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<TableDto>($"api/tables/{id}");
    }

    public async Task ReserveTableAsync(Guid id, string customerName, string customerPhone, DateTime reservationTime, int numberOfGuests)
    {
        var command = new
        {
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            ReservationTime = reservationTime,
            NumberOfGuests = numberOfGuests
        };
        var response = await _httpClient.PutAsJsonAsync($"api/tables/{id}/reserve", command);
        response.EnsureSuccessStatusCode();
    }
}

public class CreateOrderResponse
{
    public Guid Id { get; set; }
}

public class TableDto
{
    public Guid Id { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CurrentReservation { get; set; }
}

public class CreateOrderDto
{
    public Guid RestaurantId { get; set; }
    public Guid? TableId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public Guid MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? SpecialInstructions { get; set; }
    public List<OrderItemCustomizationDto> Customizations { get; set; } = new();
}

public class OrderItemCustomizationDto
{
    public string IngredientName { get; set; } = string.Empty;
    public int CustomizationType { get; set; } // 0 = More, 1 = Less
    public string? Notes { get; set; }
}
