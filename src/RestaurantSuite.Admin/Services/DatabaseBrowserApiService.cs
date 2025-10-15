using System.Net.Http.Json;
using RestaurantSuite.Application.DTOs.DatabaseBrowser;

namespace RestaurantSuite.Admin.Services;

public class DatabaseBrowserApiService
{
    private readonly HttpClient _httpClient;

    public DatabaseBrowserApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DatabaseTableInfo>> GetDatabaseTablesAsync()
    {
        var tmp = await _httpClient.GetFromJsonAsync<List<DatabaseTableInfo>>("api/admin/database/tables");
        return tmp ?? new List<DatabaseTableInfo>();
    }

    public async Task<TableSchemaResponse?> GetTableSchemaAsync(string tableName)
    {
        return await _httpClient.GetFromJsonAsync<TableSchemaResponse>(
            $"api/admin/database/tables/{Uri.EscapeDataString(tableName)}/schema");
    }

    public async Task<TableDataResponse?> GetTableDataAsync(
        string tableName,
        int page = 1,
        int pageSize = 50,
        string? sortBy = null,
        string sortDirection = "asc",
        string? searchTerm = null)
    {
        var queryParams = new List<string>
        {
            $"page={page}",
            $"pageSize={pageSize}"
        };

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            queryParams.Add($"sortBy={Uri.EscapeDataString(sortBy)}");
            queryParams.Add($"sortDirection={sortDirection}");
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
        }

        var queryString = "?" + string.Join("&", queryParams);
        return await _httpClient.GetFromJsonAsync<TableDataResponse>(
            $"api/admin/database/tables/{Uri.EscapeDataString(tableName)}/data{queryString}");
    }
}
