using System.Net.Http.Json;
using RestaurantSuite.Chef.Models;
using Blazored.LocalStorage;

namespace RestaurantSuite.Chef.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private const string ACCESS_TOKEN_KEY = "accessToken";
    private const string REFRESH_TOKEN_KEY = "refreshToken";
    private const string USER_INFO_KEY = "userInfo";

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (result?.Success == true)
            {
                // Validate that the user is a chef
                if (result.Role != "Chef")
                {
                    return new AuthResponse
                    {
                        Success = false,
                        ErrorMessage = "Only chefs can access this application"
                    };
                }

                await StoreAuthDataAsync(result);
            }

            return result ?? new AuthResponse { Success = false, ErrorMessage = "Unknown error occurred" };
        }
        catch (Exception ex)
        {
            return new AuthResponse { Success = false, ErrorMessage = $"Login failed: {ex.Message}" };
        }
    }

    public async Task<AuthResponse?> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await GetRefreshTokenAsync();
            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            var request = new RefreshTokenRequest { RefreshToken = refreshToken };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/refresh", request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (result?.Success == true)
            {
                await StoreAuthDataAsync(result);
            }

            return result;
        }
        catch
        {
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            var refreshToken = await GetRefreshTokenAsync();
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var request = new RefreshTokenRequest { RefreshToken = refreshToken };
                await _httpClient.PostAsJsonAsync("/api/auth/logout", request);
            }
        }
        catch
        {
            // Ignore errors during logout API call
        }
        finally
        {
            await ClearAuthDataAsync();
        }
    }

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        try
        {
            var token = await GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("/api/auth/me");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
            return result?.User;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            return await _localStorage.GetItemAsync<string>(ACCESS_TOKEN_KEY);
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        try
        {
            return await _localStorage.GetItemAsync<string>(REFRESH_TOKEN_KEY);
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserInfo?> GetStoredUserInfoAsync()
    {
        try
        {
            return await _localStorage.GetItemAsync<UserInfo>(USER_INFO_KEY);
        }
        catch
        {
            return null;
        }
    }

    private async Task StoreAuthDataAsync(AuthResponse authResponse)
    {
        if (!string.IsNullOrEmpty(authResponse.AccessToken))
        {
            await _localStorage.SetItemAsync(ACCESS_TOKEN_KEY, authResponse.AccessToken);
        }

        if (!string.IsNullOrEmpty(authResponse.RefreshToken))
        {
            await _localStorage.SetItemAsync(REFRESH_TOKEN_KEY, authResponse.RefreshToken);
        }

        if (authResponse.UserId.HasValue)
        {
            var userInfo = new UserInfo
            {
                Id = authResponse.UserId.Value,
                Email = authResponse.Email ?? string.Empty,
                FullName = authResponse.FullName ?? string.Empty,
                Role = authResponse.Role ?? "Chef"
            };
            await _localStorage.SetItemAsync(USER_INFO_KEY, userInfo);
        }
    }

    private async Task ClearAuthDataAsync()
    {
        await _localStorage.RemoveItemAsync(ACCESS_TOKEN_KEY);
        await _localStorage.RemoveItemAsync(REFRESH_TOKEN_KEY);
        await _localStorage.RemoveItemAsync(USER_INFO_KEY);
    }

    private class ApiResponse
    {
        public bool Success { get; set; }
        public UserInfo? User { get; set; }
    }
}
