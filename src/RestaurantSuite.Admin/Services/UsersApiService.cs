using System.Net.Http.Json;

namespace RestaurantSuite.Admin.Services;

public class UsersApiService
{
    private readonly HttpClient _httpClient;

    public UsersApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

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
