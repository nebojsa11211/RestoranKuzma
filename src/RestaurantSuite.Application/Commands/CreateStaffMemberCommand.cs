using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to create a new staff member
/// </summary>
public class CreateStaffMemberCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public string Status { get; set; } = "Active";
    public bool IsActive { get; set; } = true;
}
