using System.Net.Http.Json;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Commands;

namespace RestaurantSuite.Admin.Services;

public class StaffApiService
{
    private readonly HttpClient _httpClient;

    public StaffApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

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
}

public class CreateStaffResponse { public Guid Id { get; set; } }
