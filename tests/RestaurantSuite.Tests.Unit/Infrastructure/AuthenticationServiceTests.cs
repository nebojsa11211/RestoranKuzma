using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;
using RestaurantSuite.Domain.Enums;
using RestaurantSuite.Infrastructure.Identity;

namespace RestaurantSuite.Tests.Unit.Infrastructure;

public class AuthenticationServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "test-secret-key-minimum-32-characters-long-for-security",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:AccessTokenExpirationMinutes"] = "60",
                ["Jwt:RefreshTokenExpirationDays"] = "7"
            })
            .Build();

        _authenticationService = new JwtAuthenticationService(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            configuration);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldCreateUserAndReturnTokens()
    {
        // Arrange
        var email = "test@restaurant.com";
        var password = "SecurePass123!";
        var fullName = "John Doe";
        var role = "Waiter";

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _authenticationService.RegisterAsync(email, password, fullName, role);

        // Assert
        result.Success.Should().BeTrue();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().NotBeNull();
        result.UserId.Should().NotBeEmpty();

        _userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u =>
            u.Email == email.ToLowerInvariant() &&
            u.FullName == fullName), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnFailure()
    {
        // Arrange
        var email = "existing@restaurant.com";
        var existingUser = User.Create(email, "Existing", "User", UserRole.Waiter);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _authenticationService.RegisterAsync(email, "password", "New User", "Waiter");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("already exists");
        result.AccessToken.Should().BeNull();

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        var email = "user@restaurant.com";
        var password = "ValidPass123!";
        var user = User.Create(email, "Test", "User", UserRole.Waiter);

        // Hash the password using the same method as JwtAuthenticationService
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        var passwordHash = Convert.ToBase64String(hashedBytes);
        user.SetPasswordHash(passwordHash);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.Update(It.IsAny<User>()));

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _authenticationService.LoginAsync(email, password);

        // Assert
        result.Success.Should().BeTrue();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().NotBeNull();
        result.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldReturnFailure()
    {
        // Arrange
        var email = "nonexistent@restaurant.com";

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authenticationService.LoginAsync(email, "password");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Invalid");
        result.AccessToken.Should().BeNull();
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidToken_ShouldReturnNewTokens()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";
        var user = User.Create("test@restaurant.com", "Test", "User", UserRole.Waiter);
        user.SetPasswordHash("hash");
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

        _userRepositoryMock
            .Setup(x => x.GetByRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.Update(It.IsAny<User>()));

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _authenticationService.RefreshTokenAsync(refreshToken);

        // Assert
        result.Success.Should().BeTrue();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshTokenAsync_WithInvalidToken_ShouldReturnFailure()
    {
        // Arrange
        var refreshToken = "invalid-token";

        _userRepositoryMock
            .Setup(x => x.GetByRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authenticationService.RefreshTokenAsync(refreshToken);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Invalid");
    }

    [Fact]
    public async Task RevokeTokenAsync_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";
        var user = User.Create("test@restaurant.com", "Test", "User", UserRole.Waiter);
        user.SetPasswordHash("hash");
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

        _userRepositoryMock
            .Setup(x => x.GetByRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.Update(It.IsAny<User>()));

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _authenticationService.RevokeTokenAsync(refreshToken);

        // Assert
        result.Should().BeTrue();
    }
}
