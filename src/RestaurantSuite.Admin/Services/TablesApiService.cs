using System.Net.Http.Json;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Commands;

namespace RestaurantSuite.Admin.Services;

public class TablesApiService
{
    private readonly HttpClient _httpClient;

    public TablesApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TableDto>> GetTablesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TableDto>>("api/tables") ?? new List<TableDto>();
    }

    public async Task<TableDto?> GetTableByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<TableDto>($"api/tables/{id}");
    }

    public async Task<Guid> CreateTableAsync(CreateTableCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync("api/tables", command);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateTableResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async Task ReserveTableAsync(Guid id)
    {
        var response = await _httpClient.PutAsync($"api/tables/{id}/reserve", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task OccupyTableAsync(Guid id)
    {
        var response = await _httpClient.PutAsync($"api/tables/{id}/occupy", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task MakeTableAvailableAsync(Guid id)
    {
        var response = await _httpClient.PutAsync($"api/tables/{id}/available", null);
        response.EnsureSuccessStatusCode();
    }
}

public class CreateTableResponse { public Guid Id { get; set; } }
