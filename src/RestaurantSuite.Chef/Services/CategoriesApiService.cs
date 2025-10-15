using System.Net.Http.Json;
using System.Net.Http.Headers;
using RestaurantSuite.Chef.Models;

namespace RestaurantSuite.Chef.Services;

public class CategoriesApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public CategoriesApiService(HttpClient httpClient, AuthService authService)
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

    public async Task<CategoriesResponse> GetCategoriesAsync()
    {
        try
        {
            await EnsureAuthenticatedAsync();
            var response = await _httpClient.GetAsync("api/categories");

            if (response.IsSuccessStatusCode)
            {
                var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
                return new CategoriesResponse
                {
                    Success = true,
                    Categories = categories ?? new List<Category>()
                };
            }

            return new CategoriesResponse
            {
                Success = false,
                ErrorMessage = $"Failed to fetch categories: {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            return new CategoriesResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
