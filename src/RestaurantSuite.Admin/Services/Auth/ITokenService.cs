namespace RestaurantSuite.Admin.Services.Auth;

public interface ITokenService
{
    Task SetTokenAsync(string accessToken, string? refreshToken = null, DateTime? expiresAt = null);
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task<DateTime?> GetExpirationAsync();
    Task<bool> IsTokenExpiredAsync();
    Task ClearTokensAsync();
}
