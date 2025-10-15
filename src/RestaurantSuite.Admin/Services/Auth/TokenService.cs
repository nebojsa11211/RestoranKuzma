using Microsoft.JSInterop;

namespace RestaurantSuite.Admin.Services.Auth;

public class TokenService : ITokenService
{
    private readonly IJSRuntime _jsRuntime;
    private const string ACCESS_TOKEN_KEY = "accessToken";
    private const string REFRESH_TOKEN_KEY = "refreshToken";
    private const string EXPIRATION_KEY = "tokenExpiration";

    public TokenService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetTokenAsync(string accessToken, string? refreshToken = null, DateTime? expiresAt = null)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", ACCESS_TOKEN_KEY, accessToken);

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", REFRESH_TOKEN_KEY, refreshToken);
        }

        if (expiresAt.HasValue)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", EXPIRATION_KEY, expiresAt.Value.ToString("O"));
        }
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", ACCESS_TOKEN_KEY);
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
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", REFRESH_TOKEN_KEY);
        }
        catch
        {
            return null;
        }
    }

    public async Task<DateTime?> GetExpirationAsync()
    {
        try
        {
            var expirationStr = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", EXPIRATION_KEY);
            if (string.IsNullOrEmpty(expirationStr))
                return null;

            if (DateTime.TryParse(expirationStr, out var expiration))
                return expiration;

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsTokenExpiredAsync()
    {
        var expiration = await GetExpirationAsync();
        if (!expiration.HasValue)
            return true;

        return expiration.Value <= DateTime.UtcNow;
    }

    public async Task ClearTokensAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", ACCESS_TOKEN_KEY);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", REFRESH_TOKEN_KEY);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", EXPIRATION_KEY);
    }
}
