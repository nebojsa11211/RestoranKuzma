using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Application.Queries;
using RestaurantSuite.Domain.Entities;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Api.Controllers;

public class CreateStaffResponse
{
    public Guid Id { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StaffController(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<List<StaffMemberDto>>> GetStaffMembers(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByRoleAsync(UserRole.Waiter, cancellationToken);
        var staffMembers = users.Select(MapUserToStaffMemberDto).ToList();
        
        // Also get chefs and admin staff
        var chefs = await _userRepository.GetByRoleAsync(UserRole.Chef, cancellationToken);
        staffMembers.AddRange(chefs.Select(MapUserToStaffMemberDto));
        
        var admins = await _userRepository.GetByRoleAsync(UserRole.Admin, cancellationToken);
        staffMembers.AddRange(admins.Select(MapUserToStaffMemberDto));
        
        return Ok(staffMembers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StaffMemberDto>> GetStaffMemberById(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound();
        }
        
        return Ok(MapUserToStaffMemberDto(user));
    }

    [HttpPost]
    public async Task<ActionResult<CreateStaffResponse>> CreateStaffMember([FromBody] CreateStaffMemberCommand command, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"=== API: CreateStaffMember STARTED ===");
        Console.WriteLine($"Received command: FirstName='{command.FirstName}', LastName='{command.LastName}', Email='{command.Email}', Phone='{command.Phone}', Role='{command.Role}', Status='{command.Status}', IsActive='{command.IsActive}'");

        try
        {
            // Validate the command
            if (string.IsNullOrWhiteSpace(command.FirstName) || 
                string.IsNullOrWhiteSpace(command.LastName) || 
                string.IsNullOrWhiteSpace(command.Email))
            {
                Console.WriteLine("❌ API Validation failed: Missing required fields");
                return BadRequest("First Name, Last Name, and Email are required");
            }

            Console.WriteLine("✅ API Validation passed");

            // Create a new user with a temporary password hash (should be set properly in production)
            Console.WriteLine("🔄 Creating User entity...");
            var user = RestaurantSuite.Domain.Entities.User.Create(command.Email, command.FirstName, command.LastName, command.Role);
            user.SetPasswordHash("temp_hash"); // This should be replaced with proper password handling
            
            // Handle activation status based on IsActive from command
            if (!command.IsActive)
            {
                user.Deactivate();
            }
            
            Console.WriteLine("💾 Adding user to repository...");
            await _userRepository.AddAsync(user, cancellationToken);
            
            Console.WriteLine("💾 Saving changes to database...");
            var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"✅ Database save completed. Rows affected: {saveResult}");

            var response = new CreateStaffResponse
            {
                Id = user.Id
            };

            Console.WriteLine($"🎉 Staff member created successfully with ID: {user.Id}");
            Console.WriteLine("=== API: CreateStaffMember FINISHED ===");

            return CreatedAtAction(nameof(GetStaffMemberById), new { id = user.Id }, response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ API Error creating staff member: {ex.Message}");
            Console.WriteLine($"❌ Exception Type: {ex.GetType().FullName}");
            Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            Console.WriteLine("=== API: CreateStaffMember FAILED ===");
            return StatusCode(500, "An error occurred while creating the staff member");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStaffMember(Guid id, [FromBody] UpdateStaffMemberCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound();
        }

        user.UpdateProfile(command.FirstName, command.LastName, command.Phone);
        
        // Update role if it has changed
        if (user.Role != command.Role)
        {
            // This would require a method to change role in the User entity
            // For now, we'll update other properties
        }
        
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaffMember(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound();
        }

        user.Deactivate();
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private StaffMemberDto MapUserToStaffMemberDto(User user)
    {
        return new StaffMemberDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone ?? string.Empty,
            Role = user.Role,
            EmployeeId = $"EMP{user.Id.ToString().Substring(0, 8).ToUpper()}", // Generate employee ID from user ID
            Status = "Online", // Default status for new users - UI expects Online/On Break/Off Duty
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
