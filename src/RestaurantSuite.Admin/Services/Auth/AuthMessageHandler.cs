namespace RestaurantSuite.Admin.Services.Auth;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public AuthMessageHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Get the access token
        var token = await _tokenService.GetAccessTokenAsync();

        // If we have a token and it's not expired, add it to the request
        if (!string.IsNullOrEmpty(token))
        {
            var isExpired = await _tokenService.IsTokenExpiredAsync();
            if (!isExpired)
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
