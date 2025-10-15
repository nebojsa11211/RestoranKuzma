using System.Net.Http.Json;
using RestaurantSuite.Admin.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace RestaurantSuite.Admin.Services.Auth;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly CustomAuthenticationStateProvider _authStateProvider;

    public AuthService(
        HttpClient httpClient,
        ITokenService tokenService,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _authStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
    }

    public async Task<AuthResponse> LoginAsync(string email, string password, bool rememberMe = false)
    {
        try
        {
            var request = new LoginRequest
            {
                Email = email,
                Password = password,
                RememberMe = rememberMe
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new AuthResponse
                {
                    Success = false,
                    ErrorMessage = $"Login failed: {errorContent}"
                };
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (authResponse != null && authResponse.Success && !string.IsNullOrEmpty(authResponse.AccessToken))
            {
                // Store tokens
                await _tokenService.SetTokenAsync(
                    authResponse.AccessToken,
                    authResponse.RefreshToken,
                    authResponse.ExpiresAt);

                // Update authentication state
                await _authStateProvider.MarkUserAsAuthenticated(authResponse.AccessToken);
            }

            return authResponse ?? new AuthResponse { Success = false, ErrorMessage = "Invalid response from server" };
        }
        catch (Exception ex)
        {
            return new AuthResponse
            {
                Success = false,
                ErrorMessage = $"Login error: {ex.Message}"
            };
        }
    }

    public async Task<bool> LogoutAsync()
    {
        try
        {
            var refreshToken = await _tokenService.GetRefreshTokenAsync();

            if (!string.IsNullOrEmpty(refreshToken))
            {
                // Try to revoke the token on the server
                try
                {
                    await _httpClient.PostAsJsonAsync("api/auth/logout", new { refreshToken });
                }
                catch
                {
                    // If server logout fails, still clear local tokens
                }
            }

            // Clear tokens and update auth state
            await _authStateProvider.MarkUserAsLoggedOut();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<AuthResponse> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await _tokenService.GetRefreshTokenAsync();

            if (string.IsNullOrEmpty(refreshToken))
            {
                return new AuthResponse
                {
                    Success = false,
                    ErrorMessage = "No refresh token available"
                };
            }

            var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", new { refreshToken });

            if (!response.IsSuccessStatusCode)
            {
                await _authStateProvider.MarkUserAsLoggedOut();
                return new AuthResponse
                {
                    Success = false,
                    ErrorMessage = "Token refresh failed"
                };
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (authResponse != null && authResponse.Success && !string.IsNullOrEmpty(authResponse.AccessToken))
            {
                await _tokenService.SetTokenAsync(
                    authResponse.AccessToken,
                    authResponse.RefreshToken,
                    authResponse.ExpiresAt);

                await _authStateProvider.MarkUserAsAuthenticated(authResponse.AccessToken);
            }

            return authResponse ?? new AuthResponse { Success = false, ErrorMessage = "Invalid response" };
        }
        catch (Exception ex)
        {
            await _authStateProvider.MarkUserAsLoggedOut();
            return new AuthResponse
            {
                Success = false,
                ErrorMessage = $"Refresh error: {ex.Message}"
            };
        }
    }
}
