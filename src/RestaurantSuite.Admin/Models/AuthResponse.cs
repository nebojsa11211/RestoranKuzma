namespace RestaurantSuite.Admin.Models;

public class AuthResponse
{
    public bool Success { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Guid? UserId { get; set; }
    public string? Role { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? ErrorMessage { get; set; }
}
