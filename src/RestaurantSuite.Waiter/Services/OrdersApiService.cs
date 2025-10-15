using System.Net.Http.Json;
using System.Net.Http.Headers;
using RestaurantSuite.Waiter.Models;

namespace RestaurantSuite.Waiter.Services;

public class OrdersApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public OrdersApiService(HttpClient httpClient, AuthService authService)
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

    public async Task<OrderListResponse> GetOrdersAsync(OrderStatus? status = null)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var queryString = status.HasValue ? $"?status={status}" : "";
            var response = await _httpClient.GetAsync($"api/orders{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var orders = await response.Content.ReadFromJsonAsync<List<Order>>();
                return new OrderListResponse
                {
                    Success = true,
                    Orders = orders ?? new List<Order>()
                };
            }

            return new OrderListResponse
            {
                Success = false,
                ErrorMessage = $"Failed to fetch orders: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetOrdersAsync: {ex.Message}");
            return new OrderListResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<OrderResponse> GetOrderByIdAsync(Guid orderId)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.GetAsync($"api/orders/{orderId}");

            if (response.IsSuccessStatusCode)
            {
                var order = await response.Content.ReadFromJsonAsync<Order>();
                return new OrderResponse
                {
                    Success = true,
                    Order = order
                };
            }

            return new OrderResponse
            {
                Success = false,
                ErrorMessage = $"Failed to fetch order: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new OrderResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.PostAsJsonAsync("api/orders", request);

            if (response.IsSuccessStatusCode)
            {
                var order = await response.Content.ReadFromJsonAsync<Order>();
                return new OrderResponse
                {
                    Success = true,
                    Order = order
                };
            }

            return new OrderResponse
            {
                Success = false,
                ErrorMessage = $"Failed to create order: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new OrderResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<OrderResponse> UpdateOrderStatusAsync(UpdateOrderStatusRequest request)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/orders/{request.OrderId}/status", request);

            if (response.IsSuccessStatusCode)
            {
                var order = await response.Content.ReadFromJsonAsync<Order>();
                return new OrderResponse
                {
                    Success = true,
                    Order = order
                };
            }

            return new OrderResponse
            {
                Success = false,
                ErrorMessage = $"Failed to update order status: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new OrderResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
