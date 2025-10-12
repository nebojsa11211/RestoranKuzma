using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace RestaurantSuite.Chef.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthService _authService;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(AuthService authService)
    {
        _authService = authService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _authService.GetAccessTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(_anonymous);
            }

            // Validate token expiration
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                return new AuthenticationState(_anonymous);
            }

            var jwtToken = handler.ReadJwtToken(token);

            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                // Token expired, try to refresh
                var refreshResult = await _authService.RefreshTokenAsync();

                if (refreshResult?.Success == true && !string.IsNullOrEmpty(refreshResult.AccessToken))
                {
                    token = refreshResult.AccessToken;
                    jwtToken = handler.ReadJwtToken(token);
                }
                else
                {
                    // Refresh failed, user needs to log in again
                    await _authService.LogoutAsync();
                    return new AuthenticationState(_anonymous);
                }
            }

            // Extract claims from JWT
            var claims = jwtToken.Claims.ToList();

            // Ensure we have the required claims
            if (!claims.Any(c => c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.Sub))
            {
                return new AuthenticationState(_anonymous);
            }

            // Validate that the user is a chef
            var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (roleClaim == null || roleClaim.Value != "Chef")
            {
                await _authService.LogoutAsync();
                return new AuthenticationState(_anonymous);
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var claims = jwtToken.Claims;
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        var authState = Task.FromResult(new AuthenticationState(user));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(new AuthenticationState(_anonymous));
        NotifyAuthenticationStateChanged(authState);
    }
}
