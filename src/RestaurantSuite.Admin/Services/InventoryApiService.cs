using System.Net.Http.Json;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Commands;

namespace RestaurantSuite.Admin.Services;

public class InventoryApiService
{
    private readonly HttpClient _httpClient;

    public InventoryApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<InventoryItemDto>> GetInventoryItemsAsync(bool? lowStockOnly = null, bool includeInactive = false)
    {
        var queryParams = new List<string>();
        if (lowStockOnly.HasValue) queryParams.Add($"lowStockOnly={lowStockOnly.Value}");
        if (includeInactive) queryParams.Add($"includeInactive={includeInactive}");

        var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        return await _httpClient.GetFromJsonAsync<List<InventoryItemDto>>($"api/admin/inventory{queryString}") ?? new List<InventoryItemDto>();
    }

    public async Task<InventoryItemDto?> GetInventoryItemByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<InventoryItemDto>($"api/admin/inventory/{id}");
    }

    public async Task<List<InventoryItemDto>> GetLowStockItemsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<InventoryItemDto>>("api/admin/inventory/low-stock") ?? new List<InventoryItemDto>();
    }

    public async Task<List<StockTransactionDto>> GetStockTransactionHistoryAsync(
        Guid? inventoryItemId = null,
        Guid? orderId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int limit = 100)
    {
        var queryParams = new List<string>();
        if (inventoryItemId.HasValue) queryParams.Add($"inventoryItemId={inventoryItemId.Value}");
        if (orderId.HasValue) queryParams.Add($"orderId={orderId.Value}");
        if (fromDate.HasValue) queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-ddTHH:mm:ss}");
        if (toDate.HasValue) queryParams.Add($"toDate={toDate.Value:yyyy-MM-ddTHH:mm:ss}");
        queryParams.Add($"limit={limit}");

        var queryString = "?" + string.Join("&", queryParams);
        return await _httpClient.GetFromJsonAsync<List<StockTransactionDto>>($"api/admin/inventory/transactions{queryString}") ?? new List<StockTransactionDto>();
    }

    public async Task<Guid> CreateInventoryItemAsync(CreateInventoryItemCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync("api/admin/inventory", command);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateInventoryItemResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async Task UpdateInventoryItemAsync(Guid id, UpdateInventoryItemCommand command)
    {
        command.Id = id;
        var response = await _httpClient.PutAsJsonAsync($"api/admin/inventory/{id}", command);
        response.EnsureSuccessStatusCode();
    }

    public async Task AdjustInventoryStockAsync(Guid id, AdjustInventoryStockCommand command)
    {
        command.InventoryItemId = id;
        var response = await _httpClient.PostAsJsonAsync($"api/admin/inventory/{id}/adjust", command);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeactivateInventoryItemAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/admin/inventory/{id}");
        response.EnsureSuccessStatusCode();
    }
}

public class CreateInventoryItemResponse { public Guid Id { get; set; } }
