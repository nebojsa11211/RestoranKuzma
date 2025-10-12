using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all users (including guests) - Admin only
    /// </summary>
    [HttpGet]
    [AllowAnonymous] // Temporarily allow anonymous for testing, should be [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<object>>> GetAllUsers()
    {
        try
        {
            // Get all users from database
            var allUsers = await _userRepository.GetByRestaurantIdAsync(Guid.Empty); // Single restaurant, so no filtering

            var usersList = allUsers.Select(u => new
            {
                id = u.Id,
                email = u.Email,
                firstName = u.FirstName,
                lastName = u.LastName,
                fullName = u.FullName,
                phone = u.Phone,
                role = u.Role.ToString(),
                isActive = u.IsActive,
                createdAt = u.CreatedAt,
                updatedAt = u.UpdatedAt
            }).OrderBy(u => u.role).ThenBy(u => u.fullName).ToList();

            return Ok(usersList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all users");
            return StatusCode(500, new { success = false, message = "An error occurred while fetching users" });
        }
    }

    /// <summary>
    /// Get users by role
    /// </summary>
    [HttpGet("by-role/{role}")]
    [AllowAnonymous] // Temporarily allow anonymous for testing
    public async Task<ActionResult<List<object>>> GetUsersByRole(string role)
    {
        try
        {
            if (!Enum.TryParse<Domain.Enums.UserRole>(role, true, out var userRole))
            {
                return BadRequest(new { success = false, message = "Invalid role specified" });
            }

            var users = await _userRepository.GetByRoleAsync(userRole);

            var usersList = users.Select(u => new
            {
                id = u.Id,
                email = u.Email,
                firstName = u.FirstName,
                lastName = u.LastName,
                fullName = u.FullName,
                phone = u.Phone,
                role = u.Role.ToString(),
                isActive = u.IsActive,
                createdAt = u.CreatedAt
            }).ToList();

            return Ok(usersList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users by role {Role}", role);
            return StatusCode(500, new { success = false, message = "An error occurred while fetching users" });
        }
    }

    /// <summary>
    /// Get count of users by role
    /// </summary>
    [HttpGet("count")]
    [AllowAnonymous] // Temporarily allow anonymous for testing
    public async Task<ActionResult<object>> GetUserCounts()
    {
        try
        {
            var allUsers = await _userRepository.GetByRestaurantIdAsync(Guid.Empty);

            var counts = new
            {
                total = allUsers.Count(),
                admin = allUsers.Count(u => u.Role == Domain.Enums.UserRole.Admin),
                waiter = allUsers.Count(u => u.Role == Domain.Enums.UserRole.Waiter),
                chef = allUsers.Count(u => u.Role == Domain.Enums.UserRole.Chef),
                guest = allUsers.Count(u => u.Role == Domain.Enums.UserRole.Guest),
                active = allUsers.Count(u => u.IsActive),
                inactive = allUsers.Count(u => !u.IsActive)
            };

            return Ok(counts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user counts");
            return StatusCode(500, new { success = false, message = "An error occurred while fetching user counts" });
        }
    }
}
