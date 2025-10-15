using System.Net.Http.Json;
using System.Net.Http.Headers;
using RestaurantSuite.Chef.Models;

namespace RestaurantSuite.Chef.Services;

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

    public async Task<OrderResponse> StartPreparationAsync(Guid orderId)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var request = new UpdateOrderStatusRequest
            {
                OrderId = orderId,
                Action = "start-preparation"
            };
            var response = await _httpClient.PutAsJsonAsync($"api/orders/{orderId}/status", request);

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
                ErrorMessage = $"Failed to start preparation: {response.ReasonPhrase}"
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

    public async Task<OrderResponse> MarkAsReadyAsync(Guid orderId)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var request = new UpdateOrderStatusRequest
            {
                OrderId = orderId,
                Action = "complete-preparation"
            };
            var response = await _httpClient.PutAsJsonAsync($"api/orders/{orderId}/status", request);

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
                ErrorMessage = $"Failed to mark as ready: {response.ReasonPhrase}"
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

    public async Task<KitchenStatsResponse> GetKitchenStatsAsync()
    {
        try
        {
            await EnsureAuthenticatedAsync();

            // Get all orders and calculate stats on client side
            var allOrdersResponse = await GetOrdersAsync();

            if (!allOrdersResponse.Success)
            {
                return new KitchenStatsResponse
                {
                    Success = false,
                    ErrorMessage = allOrdersResponse.ErrorMessage
                };
            }

            var orders = allOrdersResponse.Orders ?? new List<Order>();
            var today = DateTime.UtcNow.Date;

            var stats = new KitchenStats
            {
                ConfirmedOrders = orders.Count(o => o.Status == OrderStatus.Confirmed),
                InProgressOrders = orders.Count(o => o.Status == OrderStatus.InProgress),
                ReadyOrders = orders.Count(o => o.Status == OrderStatus.Ready),
                CompletedToday = orders.Count(o => o.Status == OrderStatus.Completed &&
                                                    o.CompletedAt.HasValue &&
                                                    o.CompletedAt.Value.Date == today)
            };

            return new KitchenStatsResponse
            {
                Success = true,
                Stats = stats
            };
        }
        catch (Exception ex)
        {
            return new KitchenStatsResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
