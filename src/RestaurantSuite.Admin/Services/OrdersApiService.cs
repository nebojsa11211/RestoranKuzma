using System.Net.Http.Json;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Commands;

namespace RestaurantSuite.Admin.Services;

public class OrdersApiService
{
    private readonly HttpClient _httpClient;

    public OrdersApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<OrderDto>> GetOrdersByRestaurantAsync(Guid restaurantId)
    {
        return await _httpClient.GetFromJsonAsync<List<OrderDto>>($"api/orders/restaurant/{restaurantId}") ?? new List<OrderDto>();
    }

    public async Task<Guid> CreateOrderAsync(CreateOrderCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync("api/orders", command);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async Task UpdateOrderStatusAsync(Guid id, UpdateOrderStatusCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/orders/{id}/status", command);
        response.EnsureSuccessStatusCode();
    }
}

public class CreateOrderResponse { public Guid Id { get; set; } }
