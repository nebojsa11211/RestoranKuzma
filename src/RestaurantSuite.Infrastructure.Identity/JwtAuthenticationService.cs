using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Infrastructure.Identity;

public class JwtAuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
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
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
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
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return AuthenticationResult.Failure("Invalid email or password");
        }

        if (!VerifyPassword(password, user.PasswordHash))
        {
            return AuthenticationResult.Failure("Invalid email or password");
        }

        if (!user.IsActive)
        {
            return AuthenticationResult.Failure("User account is deactivated");
        }

        // Generate new tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        // Use extended expiration if "Remember Me" is checked
        var refreshTokenExpirationDays = rememberMe ? _rememberMeRefreshTokenExpirationDays : _refreshTokenExpirationDays;
        var refreshTokenExpires = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);
        user.SetRefreshToken(refreshToken, refreshTokenExpires);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

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

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

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
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

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
