using System.Net.Http.Json;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Commands;

namespace RestaurantSuite.Admin.Services;

public class CategoriesApiService
{
    private readonly HttpClient _httpClient;

    public CategoriesApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/categories") ?? new List<CategoryDto>();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<CategoryDto>($"api/categories/{id}");
    }

    public async Task<Guid> CreateCategoryAsync(CreateCategoryCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync("api/categories", command);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async Task UpdateCategoryAsync(Guid id, UpdateCategoryCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/categories/{id}", command);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/categories/{id}");
        response.EnsureSuccessStatusCode();
    }
}

public class CreateCategoryResponse { public Guid Id { get; set; } }
