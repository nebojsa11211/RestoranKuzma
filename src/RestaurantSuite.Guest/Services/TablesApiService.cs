using System.Net.Http.Json;

namespace RestaurantSuite.Guest.Services;

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

public class TableDto
{
    public Guid Id { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CurrentReservation { get; set; }
}
