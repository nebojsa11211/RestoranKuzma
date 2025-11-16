using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using RestaurantSuite.Waiter;
using RestaurantSuite.Waiter.Services;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient for API communication
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl") ?? "http://localhost:5213";

// Register LocalStorage service
builder.Services.AddBlazoredLocalStorage();

// Register Authentication Services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthenticationStateProvider>());

// Register AuthMessageHandler for automatic token injection
builder.Services.AddScoped<AuthMessageHandler>();

// Register HttpClient with AuthMessageHandler
builder.Services.AddScoped(sp =>
{
    var authHandler = sp.GetRequiredService<AuthMessageHandler>();
    authHandler.InnerHandler = new HttpClientHandler();

    var httpClient = new HttpClient(authHandler)
    {
        BaseAddress = new Uri(apiBaseUrl)
    };

    return httpClient;
});

// Register Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<OrdersApiService>();
builder.Services.AddScoped<TablesApiService>();
builder.Services.AddScoped<MenuApiService>();
builder.Services.AddScoped<PaymentsApiService>();
builder.Services.AddScoped<InvoicesApiService>();

// Register SignalR service
builder.Services.AddScoped(sp => new SignalRService(
    sp.GetRequiredService<AuthService>(),
    apiBaseUrl
));

await builder.Build().RunAsync();
