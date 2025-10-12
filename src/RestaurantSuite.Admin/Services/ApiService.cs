using System.Net.Http.Json;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.Queries;

namespace RestaurantSuite.Admin.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Restaurants
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

    // Orders
    public async Task<List<OrderDto>> GetOrdersByRestaurantAsync(Guid restaurantId)
    {
        return await _httpClient.GetFromJsonAsync<List<OrderDto>>($"api/orders/restaurant/{restaurantId}") ?? new List<OrderDto>();
    }

    public async Task<Guid> CreateOrderAsync(CreateOrderCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync("api/orders", command);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async Task UpdateOrderStatusAsync(Guid id, UpdateOrderStatusCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/orders/{id}/status", command);
        response.EnsureSuccessStatusCode();
    }

    // Staff Management
    public async Task<List<StaffMemberDto>> GetStaffMembersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<StaffMemberDto>>("api/staff") ?? new List<StaffMemberDto>();
    }

    public async Task<StaffMemberDto?> GetStaffMemberByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<StaffMemberDto>($"api/staff/{id}");
    }

    public async Task<Guid> CreateStaffMemberAsync(CreateStaffMemberCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync("api/staff", command);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateStaffResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async Task UpdateStaffMemberAsync(Guid id, UpdateStaffMemberCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/staff/{id}", command);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteStaffMemberAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/staff/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Menu Items
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

    // Categories
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
        var response = await _httpClient.PostAsJsonAsync("api/admin/categories", command);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async Task UpdateCategoryAsync(Guid id, UpdateCategoryCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/admin/categories/{id}", command);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/admin/categories/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Tables
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

    // Users (including guests)
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<UserDto>>("api/users") ?? new List<UserDto>();
    }

    public async Task<List<UserDto>> GetUsersByRoleAsync(string role)
    {
        return await _httpClient.GetFromJsonAsync<List<UserDto>>($"api/users/by-role/{role}") ?? new List<UserDto>();
    }

    public async Task<UserCountsDto?> GetUserCountsAsync()
    {
        return await _httpClient.GetFromJsonAsync<UserCountsDto>("api/users/count");
    }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UserCountsDto
{
    public int Total { get; set; }
    public int Admin { get; set; }
    public int Waiter { get; set; }
    public int Chef { get; set; }
    public int Guest { get; set; }
    public int Active { get; set; }
    public int Inactive { get; set; }
}

public class CreateRestaurantResponse
{
    public Guid Id { get; set; }
}

public class CreateOrderResponse
{
    public Guid Id { get; set; }
}

public class CreateStaffResponse
{
    public Guid Id { get; set; }
}

public class CreateMenuItemResponse
{
    public Guid Id { get; set; }
}

public class CreateCategoryResponse
{
    public Guid Id { get; set; }
}

public class CreateTableResponse
{
    public Guid Id { get; set; }
}
