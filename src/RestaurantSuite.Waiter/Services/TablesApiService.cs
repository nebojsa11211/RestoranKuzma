using System.Net.Http.Json;
using System.Net.Http.Headers;
using RestaurantSuite.Waiter.Models;

namespace RestaurantSuite.Waiter.Services;

public class TablesApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public TablesApiService(HttpClient httpClient, AuthService authService)
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
}
