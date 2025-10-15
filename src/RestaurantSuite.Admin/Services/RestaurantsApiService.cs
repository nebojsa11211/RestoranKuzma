using System.Net.Http.Json;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Commands;

namespace RestaurantSuite.Admin.Services;

public class RestaurantsApiService
{
    private readonly HttpClient _httpClient;

    public RestaurantsApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<RestaurantDto>> GetRestaurantsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<RestaurantDto>>("api/restaurants") ?? new List<RestaurantDto>();
    }

    public async Task<RestaurantDto?> GetRestaurantByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<RestaurantDto>($"api/restaurants/{id}");
    }

    public async Task<Guid> CreateRestaurantAsync(CreateRestaurantCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync("api/restaurants", command);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateRestaurantResponse>();
        return result?.Id ?? Guid.Empty;
    }
}

public class CreateRestaurantResponse
{
    public Guid Id { get; set; }
}
