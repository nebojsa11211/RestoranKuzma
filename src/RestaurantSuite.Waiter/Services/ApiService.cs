using System.Net.Http.Json;
using System.Net.Http.Headers;
using RestaurantSuite.Waiter.Models;

namespace RestaurantSuite.Waiter.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public ApiService(HttpClient httpClient, AuthService authService)
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

    // Orders
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

    // Tables
    public async Task<TableListResponse> GetTablesAsync()
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.GetAsync("api/tables");

            if (response.IsSuccessStatusCode)
            {
                var tables = await response.Content.ReadFromJsonAsync<List<Table>>();
                return new TableListResponse
                {
                    Success = true,
                    Tables = tables ?? new List<Table>()
                };
            }

            return new TableListResponse
            {
                Success = false,
                ErrorMessage = $"Failed to fetch tables: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new TableListResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<TableResponse> GetTableByIdAsync(Guid tableId)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.GetAsync($"api/tables/{tableId}");

            if (response.IsSuccessStatusCode)
            {
                var table = await response.Content.ReadFromJsonAsync<Table>();
                return new TableResponse
                {
                    Success = true,
                    Table = table
                };
            }

            return new TableResponse
            {
                Success = false,
                ErrorMessage = $"Failed to fetch table: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new TableResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<TableResponse> UpdateTableStatusAsync(UpdateTableStatusRequest request)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/tables/{request.TableId}/status", request);

            if (response.IsSuccessStatusCode)
            {
                var table = await response.Content.ReadFromJsonAsync<Table>();
                return new TableResponse
                {
                    Success = true,
                    Table = table
                };
            }

            return new TableResponse
            {
                Success = false,
                ErrorMessage = $"Failed to update table status: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new TableResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    // Menu Items
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

    // Payments
    public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.PostAsJsonAsync("api/payments", request);

            if (response.IsSuccessStatusCode)
            {
                var payment = await response.Content.ReadFromJsonAsync<Payment>();
                return new PaymentResponse
                {
                    Success = true,
                    Payment = payment
                };
            }

            return new PaymentResponse
            {
                Success = false,
                ErrorMessage = $"Failed to create payment: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new PaymentResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<PaymentListResponse> ProcessSplitPaymentAsync(SplitPaymentRequest request)
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.PostAsJsonAsync("api/payments/split", request);

            if (response.IsSuccessStatusCode)
            {
                var payments = await response.Content.ReadFromJsonAsync<List<Payment>>();
                return new PaymentListResponse
                {
                    Success = true,
                    Payments = payments ?? new List<Payment>()
                };
            }

            return new PaymentListResponse
            {
                Success = false,
                ErrorMessage = $"Failed to process split payment: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new PaymentListResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}

// Additional DTOs used by ApiService
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
