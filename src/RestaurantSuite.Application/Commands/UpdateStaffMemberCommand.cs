using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to update an existing staff member
/// </summary>
public class UpdateStaffMemberCommand
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public string Status { get; set; } = "Active";
    public bool IsActive { get; set; } = true;
}
