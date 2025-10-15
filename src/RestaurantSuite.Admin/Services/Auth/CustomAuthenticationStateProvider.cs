using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace RestaurantSuite.Admin.Services.Auth;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenService _tokenService;
    private readonly JwtSecurityTokenHandler _jwtHandler = new();

    public CustomAuthenticationStateProvider(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenService.GetAccessTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Check if token is expired
        var isExpired = await _tokenService.IsTokenExpiredAsync();
        if (isExpired)
        {
            await _tokenService.ClearTokensAsync();
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Parse JWT token to get claims
        try
        {
            var jwtToken = _jwtHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

            // Create authenticated user
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            // Invalid token
            await _tokenService.ClearTokensAsync();
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async Task MarkUserAsAuthenticated(string accessToken)
    {
        var jwtToken = _jwtHandler.ReadJwtToken(accessToken);
        var claims = jwtToken.Claims.ToList();

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _tokenService.ClearTokensAsync();

        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
    }
}
