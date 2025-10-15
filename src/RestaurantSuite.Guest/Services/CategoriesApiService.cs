using System.Net.Http.Json;
using RestaurantSuite.Guest.Models;

namespace RestaurantSuite.Guest.Services;

public class CategoriesApiService
{
    private readonly HttpClient _httpClient;

    public CategoriesApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/categories");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
                return content ?? new List<CategoryDto>();
            }
            else
            {
                Console.WriteLine($"API Error in GetCategoriesAsync: {response.StatusCode} - {response.ReasonPhrase}");
                return new List<CategoryDto>();
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Request Error in GetCategoriesAsync: {ex.Message}");
            Console.WriteLine($"Base Address: {_httpClient.BaseAddress}");
            return new List<CategoryDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error in GetCategoriesAsync: {ex.Message}");
            return new List<CategoryDto>();
        }
    }
}
