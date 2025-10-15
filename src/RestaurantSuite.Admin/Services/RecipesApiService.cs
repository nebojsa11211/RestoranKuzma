using System.Net.Http.Json;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Commands;

namespace RestaurantSuite.Admin.Services;

public class RecipesApiService
{
    private readonly HttpClient _httpClient;

    public RecipesApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<RecipeDto>> GetRecipesAsync(bool includeInactive = false)
    {
        var queryString = includeInactive ? "?includeInactive=true" : "";
        return await _httpClient.GetFromJsonAsync<List<RecipeDto>>($"api/admin/recipes{queryString}") ?? new List<RecipeDto>();
    }

    public async Task<RecipeDto?> GetRecipeByMenuItemIdAsync(Guid menuItemId)
    {
        return await _httpClient.GetFromJsonAsync<RecipeDto>($"api/admin/recipes/menu-item/{menuItemId}");
    }

    public async Task<Guid> CreateRecipeAsync(CreateRecipeCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync("api/admin/recipes", command);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateRecipeResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async Task UpdateRecipeAsync(Guid menuItemId, UpdateRecipeCommand command)
    {
        command.MenuItemId = menuItemId;
        var response = await _httpClient.PutAsJsonAsync($"api/admin/recipes/menu-item/{menuItemId}", command);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeactivateRecipeAsync(Guid menuItemId)
    {
        var response = await _httpClient.DeleteAsync($"api/admin/recipes/menu-item/{menuItemId}");
        response.EnsureSuccessStatusCode();
    }
}

public class CreateRecipeResponse { public Guid Id { get; set; } }
