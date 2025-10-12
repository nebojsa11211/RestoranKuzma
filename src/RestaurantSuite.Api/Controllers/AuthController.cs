using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Api.Models;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthenticationService authService,
        IUserRepository userRepository,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid request data"
                });
            }

            // Construct full name from first and last name
            var fullName = $"{request.FirstName} {request.LastName}";

            var result = await _authService.RegisterAsync(
                request.Email,
                request.Password,
                fullName,
                request.Role);

            if (!result.Success)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    ErrorMessage = result.ErrorMessage
                });
            }

            // Get user details for response
            var user = await _userRepository.GetByIdAsync(result.UserId!.Value);

            return Ok(new AuthResponse
            {
                Success = true,
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                ExpiresAt = result.ExpiresAt,
                UserId = result.UserId,
                Role = user?.Role.ToString(),
                Email = user?.Email,
                FullName = user?.FullName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return StatusCode(500, new AuthResponse
            {
                Success = false,
                ErrorMessage = "An error occurred during registration"
            });
        }
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid request data"
                });
            }

            var result = await _authService.LoginAsync(request.Email, request.Password, request.RememberMe);

            if (!result.Success)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    ErrorMessage = result.ErrorMessage
                });
            }

            // Get user details for response
            var user = await _userRepository.GetByIdAsync(result.UserId!.Value);

            return Ok(new AuthResponse
            {
                Success = true,
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                ExpiresAt = result.ExpiresAt,
                UserId = result.UserId,
                Role = user?.Role.ToString(),
                Email = user?.Email,
                FullName = user?.FullName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return StatusCode(500, new AuthResponse
            {
                Success = false,
                ErrorMessage = "An error occurred during login"
            });
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid request data"
                });
            }

            var result = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (!result.Success)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    ErrorMessage = result.ErrorMessage
                });
            }

            // Get user details for response
            var user = await _userRepository.GetByIdAsync(result.UserId!.Value);

            return Ok(new AuthResponse
            {
                Success = true,
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                ExpiresAt = result.ExpiresAt,
                UserId = result.UserId,
                Role = user?.Role.ToString(),
                Email = user?.Email,
                FullName = user?.FullName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new AuthResponse
            {
                Success = false,
                ErrorMessage = "An error occurred during token refresh"
            });
        }
    }

    /// <summary>
    /// Logout and revoke refresh token
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid request data" });
            }

            var success = await _authService.RevokeTokenAsync(request.RefreshToken);

            if (!success)
            {
                return BadRequest(new { success = false, message = "Failed to revoke token" });
            }

            return Ok(new { success = true, message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { success = false, message = "An error occurred during logout" });
        }
    }

    /// <summary>
    /// Get current authenticated user info
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<object>> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { success = false, message = "Invalid token" });
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found" });
            }

            return Ok(new
            {
                success = true,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    fullName = user.FullName,
                    phone = user.Phone,
                    role = user.Role.ToString(),
                    isActive = user.IsActive
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, new { success = false, message = "An error occurred" });
        }
    }
}
