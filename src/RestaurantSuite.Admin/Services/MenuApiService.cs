using System.Net.Http.Json;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Admin.Services;

public class MenuApiService
{
    private readonly HttpClient _httpClient;

    public MenuApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MenuItemDto>> GetMenuItemsAsync(Guid? categoryId = null, bool? availableOnly = null)
    {
        var queryParams = new List<string>();
        if (categoryId.HasValue) queryParams.Add($"categoryId={categoryId.Value}");
        if (availableOnly.HasValue) queryParams.Add($"availableOnly={availableOnly.Value}");
        
        var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        return await _httpClient.GetFromJsonAsync<List<MenuItemDto>>($"api/menu{queryString}") ?? new List<MenuItemDto>();
    }

    public async Task<MenuItemDto?> GetMenuItemByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<MenuItemDto>($"api/menu/{id}");
    }

    public async Task<Guid> CreateMenuItemAsync(CreateMenuItemDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/admin/menu", dto);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateMenuItemResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async Task UpdateMenuItemAsync(Guid id, UpdateMenuItemDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/admin/menu/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateMenuItemPriceAsync(Guid id, decimal price)
    {
        var response = await _httpClient.PatchAsJsonAsync($"api/admin/menu/{id}/price", price);
        response.EnsureSuccessStatusCode();
    }

    public async Task ToggleMenuItemAvailabilityAsync(Guid id, bool isAvailable)
    {
        var response = await _httpClient.PatchAsJsonAsync($"api/admin/menu/{id}/availability", isAvailable);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteMenuItemAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/admin/menu/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<MenuItemWithIngredientsDto?> GetMenuItemWithRecipeAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<MenuItemWithIngredientsDto>($"api/admin/menu/{id}/recipe");
    }

    public async Task<List<MenuItemIngredientDto>> GetIngredientsAsync(Guid menuItemId)
    {
        return await _httpClient.GetFromJsonAsync<List<MenuItemIngredientDto>>($"api/admin/menu/{menuItemId}/ingredients") ?? new List<MenuItemIngredientDto>();
    }

    public async Task<Guid> AddIngredientAsync(Guid menuItemId, CreateMenuItemIngredientDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/admin/menu/{menuItemId}/ingredients", dto);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateIngredientResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async Task UpdateIngredientAsync(Guid id, UpdateMenuItemIngredientDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/admin/ingredients/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteIngredientAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/admin/ingredients/{id}");
        response.EnsureSuccessStatusCode();
    }
}

public class CreateMenuItemResponse { public Guid Id { get; set; } }
public class CreateIngredientResponse { public Guid Id { get; set; } }
