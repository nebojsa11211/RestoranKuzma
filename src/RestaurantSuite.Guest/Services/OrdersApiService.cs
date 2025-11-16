using System.Net.Http.Json;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Guest.Services;

public class OrdersApiService
{
    private readonly HttpClient _httpClient;

    public OrdersApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Guid> CreateOrderAsync(CreateOrderDto order)
    {
        var response = await _httpClient.PostAsJsonAsync("api/orders", order);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async Task<List<OrderDto>> GetMyOrdersAsync(Guid userId)
    {
        var response = await _httpClient.GetAsync($"api/orders/user/{userId}");
        response.EnsureSuccessStatusCode();
        var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
        return orders ?? new List<OrderDto>();
    }
}

public class CreateOrderResponse
{
    public Guid Id { get; set; }
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
