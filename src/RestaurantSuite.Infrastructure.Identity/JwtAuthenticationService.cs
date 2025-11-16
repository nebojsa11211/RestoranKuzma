using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;
using RestaurantSuite.Domain.Enums;
using RestaurantSuite.Infrastructure.EF;

namespace RestaurantSuite.Infrastructure.Identity;

public class JwtAuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _accessTokenExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;
    private readonly int _rememberMeRefreshTokenExpirationDays;

    public JwtAuthenticationService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _context = context;
        _configuration = configuration;

        _jwtSecret = _configuration["Jwt:Secret"] ?? "development-secret-key-minimum-32-characters-long-for-security";
        _jwtIssuer = _configuration["Jwt:Issuer"] ?? "RestaurantSuite";
        _jwtAudience = _configuration["Jwt:Audience"] ?? "RestaurantSuite";
        _accessTokenExpirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60");
        _refreshTokenExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");
        _rememberMeRefreshTokenExpirationDays = int.Parse(_configuration["Jwt:RememberMeRefreshTokenExpirationDays"] ?? "30");
    }

    public async Task<AuthenticationResult> RegisterAsync(string email, string password, string fullName, string role)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
        {
            return AuthenticationResult.Failure("User with this email already exists");
        }

        // Parse role
        if (!Enum.TryParse<UserRole>(role, true, out var userRole))
        {
            return AuthenticationResult.Failure("Invalid role specified");
        }

        // Parse full name
        var nameParts = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (nameParts.Length < 2)
        {
            return AuthenticationResult.Failure("Full name must include first and last name");
        }

        var firstName = nameParts[0];
        var lastName = nameParts[1];

        // Create user
        var user = User.Create(email, firstName, lastName, userRole);
        var passwordHash = HashPassword(password);
        user.SetPasswordHash(passwordHash);

        // Generate tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpires = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
        user.SetRefreshToken(refreshToken, refreshTokenExpires);

        // Save to database
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return AuthenticationResult.SuccessResult(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
            user.Id);
    }

    public async Task<AuthenticationResult> LoginAsync(string email, string password, bool rememberMe = false)
    {
        Console.WriteLine($"[DEBUG] Login attempt for email: {email}");

        // Get user ID first to avoid tracking conflicts
        var userId = await _context.Users
            .Where(u => u.Email.ToLower() == email.ToLower())
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync();

        if (!userId.HasValue)
        {
            Console.WriteLine($"[DEBUG] User not found for email: {email}");
            return AuthenticationResult.Failure("Invalid email or password");
        }

        // Load user in a way that minimizes tracking conflicts
        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user == null)
        {
            Console.WriteLine($"[DEBUG] User not found for ID: {userId.Value}");
            return AuthenticationResult.Failure("Invalid email or password");
        }

        Console.WriteLine($"[DEBUG] User found: ID={user.Id}, Email={user.Email}, IsActive={user.IsActive}");
        Console.WriteLine($"[DEBUG] Provided password: '{password}' (length: {password?.Length ?? 0})");
        Console.WriteLine($"[DEBUG] Stored password hash: '{user.PasswordHash}' (length: {user.PasswordHash?.Length ?? 0})");

        bool passwordValid = VerifyPassword(password, user.PasswordHash);
        Console.WriteLine($"[DEBUG] Password verification result: {passwordValid}");
        
        if (!passwordValid)
        {
            Console.WriteLine($"[DEBUG] Password verification failed - calculating expected hash for debugging");
            string expectedHash = HashPassword(password);
            Console.WriteLine($"[DEBUG] Expected hash: '{expectedHash}'");
            Console.WriteLine($"[DEBUG] Expected hash matches stored: {expectedHash == user.PasswordHash}");
            return AuthenticationResult.Failure("Invalid email or password");
        }

        if (!user.IsActive)
        {
            Console.WriteLine($"[DEBUG] User account is deactivated");
            return AuthenticationResult.Failure("User account is deactivated");
        }

        // Generate new tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        // Use extended expiration if "Remember Me" is checked
        var refreshTokenExpirationDays = rememberMe ? _rememberMeRefreshTokenExpirationDays : _refreshTokenExpirationDays;
        var refreshTokenExpires = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

        Console.WriteLine($"[DEBUG] Before updating refresh token - User.Id={user.Id}");

        // Detach the user entity to avoid tracking conflicts
        _context.Entry(user).State = EntityState.Detached;

        // Load a fresh instance of the user
        var userForUpdate = await _userRepository.GetByIdAsync(userId.Value);
        if (userForUpdate == null)
        {
            return AuthenticationResult.Failure("User account no longer exists");
        }

        // Update the refresh token
        userForUpdate.SetRefreshToken(refreshToken, refreshTokenExpires);

        // Manually mark the properties as modified since SetRefreshToken uses private setters
        _context.Entry(userForUpdate).Property("RefreshToken").IsModified = true;
        _context.Entry(userForUpdate).Property("RefreshTokenExpiresAt").IsModified = true;
        _context.Entry(userForUpdate).Property("UpdatedAt").IsModified = true;

        var rowsAffected = await _unitOfWork.SaveChangesAsync();
        Console.WriteLine($"[DEBUG] SaveChangesAsync completed successfully. Rows affected: {rowsAffected}");

        return AuthenticationResult.SuccessResult(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
            user.Id);
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
        {
            return AuthenticationResult.Failure("Invalid refresh token");
        }

        if (!user.IsRefreshTokenValid(refreshToken))
        {
            return AuthenticationResult.Failure("Invalid or expired refresh token");
        }

        // Generate new tokens
        var accessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpires = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
        user.SetRefreshToken(newRefreshToken, refreshTokenExpires);

        // Explicitly mark the user entity as modified to ensure EF Core tracks changes
        _userRepository.Update(user);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // If concurrency exception occurs, re-fetch and try again once
            var refreshedUser = await _userRepository.GetByIdAsync(user.Id);
            if (refreshedUser == null)
            {
                return AuthenticationResult.Failure("User account no longer exists");
            }

            refreshedUser.SetRefreshToken(newRefreshToken, refreshTokenExpires);
            _userRepository.Update(refreshedUser);
            await _unitOfWork.SaveChangesAsync();
        }

        return AuthenticationResult.SuccessResult(
            accessToken,
            newRefreshToken,
            DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
            user.Id);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
        {
            return false;
        }

        user.RevokeRefreshToken();

        // Explicitly mark the user entity as modified to ensure EF Core tracks changes
        _userRepository.Update(user);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // If concurrency exception occurs, re-fetch and try again once
            var refreshedUser = await _userRepository.GetByIdAsync(user.Id);
            if (refreshedUser == null)
            {
                return false;
            }

            refreshedUser.RevokeRefreshToken();
            _userRepository.Update(refreshedUser);
            await _unitOfWork.SaveChangesAsync();
        }

        return true;
    }

    private string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private string HashPassword(string password)
    {
        // Using BCrypt for password hashing would be better, but for simplicity using SHA256
        // In production, use BCrypt.Net-Next or ASP.NET Core Identity password hasher
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        var hash = HashPassword(password);
        return hash == passwordHash;
    }
}
