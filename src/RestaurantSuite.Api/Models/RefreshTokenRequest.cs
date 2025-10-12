using System.ComponentModel.DataAnnotations;

namespace RestaurantSuite.Api.Models;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}
