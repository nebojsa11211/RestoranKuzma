namespace RestaurantSuite.Application.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResult> RegisterAsync(string email, string password, string fullName, string role);
    Task<AuthenticationResult> LoginAsync(string email, string password, bool rememberMe = false);
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);
}

public class AuthenticationResult
{
    public bool Success { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? UserId { get; set; }

    public static AuthenticationResult Failure(string errorMessage)
    {
        return new AuthenticationResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }

    public static AuthenticationResult SuccessResult(string accessToken, string refreshToken, DateTime expiresAt, Guid userId)
    {
        return new AuthenticationResult
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            UserId = userId
        };
    }
}
