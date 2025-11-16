using System.Net.Http.Json;
using RestaurantSuite.Admin.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace RestaurantSuite.Admin.Services.Auth;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly ServerSideTokenStorage _serverSideTokenStorage;
    private readonly CustomAuthenticationStateProvider _authStateProvider;
    private readonly IServiceProvider _serviceProvider;

    public AuthService(
        HttpClient httpClient,
        ITokenService tokenService,
        ServerSideTokenStorage serverSideTokenStorage,
        AuthenticationStateProvider authStateProvider,
        IServiceProvider serviceProvider)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _serverSideTokenStorage = serverSideTokenStorage;
        _authStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
        _serviceProvider = serviceProvider;
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
                // Store tokens in both client-side (localStorage) and server-side storage
                await _tokenService.SetTokenAsync(
                    authResponse.AccessToken,
                    authResponse.RefreshToken,
                    authResponse.ExpiresAt);

                // Also store in server-side storage for SignalR
                _serverSideTokenStorage.SetToken(
                    authResponse.AccessToken,
                    authResponse.RefreshToken,
                    authResponse.ExpiresAt);

                // Update authentication state
                await _authStateProvider.MarkUserAsAuthenticated(authResponse.AccessToken);

                // Reconnect ChatHub with the new authentication token
                // This ensures SignalR uses the correct user context
                await ReconnectChatHubAsync();
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

            // Disconnect ChatHub before clearing tokens
            await DisconnectChatHubAsync();

            // Clear tokens from both storages and update auth state
            _serverSideTokenStorage.ClearTokens();
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

    private async Task ReconnectChatHubAsync()
    {
        try
        {
            // Get ChatHubService from DI container
            // We use IServiceProvider instead of direct injection to avoid circular dependencies
            var chatHubService = _serviceProvider.GetService<ChatHubService>();
            if (chatHubService != null)
            {
                Console.WriteLine("[AuthService] Reconnecting ChatHub with new authentication token...");
                await chatHubService.ReconnectWithAuthenticationAsync();
                Console.WriteLine("[AuthService] ChatHub reconnection completed");
            }
        }
        catch (Exception ex)
        {
            // Don't fail login if ChatHub reconnection fails
            Console.WriteLine($"[AuthService] ChatHub reconnection failed (non-critical): {ex.Message}");
        }
    }

    private async Task DisconnectChatHubAsync()
    {
        try
        {
            var chatHubService = _serviceProvider.GetService<ChatHubService>();
            if (chatHubService != null)
            {
                Console.WriteLine("[AuthService] Disconnecting ChatHub before logout...");
                await chatHubService.DisconnectAsync();
                Console.WriteLine("[AuthService] ChatHub disconnection completed");
            }
        }
        catch (Exception ex)
        {
            // Don't fail logout if ChatHub disconnection fails
            Console.WriteLine($"[AuthService] ChatHub disconnection failed (non-critical): {ex.Message}");
        }
    }
}
