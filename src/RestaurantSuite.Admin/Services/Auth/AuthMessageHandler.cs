namespace RestaurantSuite.Admin.Services.Auth;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly ServerSideTokenStorage _serverSideTokenStorage;

    public AuthMessageHandler(ServerSideTokenStorage serverSideTokenStorage)
    {
        _serverSideTokenStorage = serverSideTokenStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Get the access token from server-side storage
        var token = _serverSideTokenStorage.GetAccessToken();

        // If we have a token and it's not expired, add it to the request
        if (!string.IsNullOrEmpty(token))
        {
            var isExpired = _serverSideTokenStorage.IsTokenExpired();
            if (!isExpired)
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine($"[AuthMessageHandler] Added Bearer token to {request.RequestUri?.PathAndQuery}");
            }
            else
            {
                Console.WriteLine($"[AuthMessageHandler] Token expired for {request.RequestUri?.PathAndQuery}");
            }
        }
        else
        {
            Console.WriteLine($"[AuthMessageHandler] No token available for {request.RequestUri?.PathAndQuery}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
