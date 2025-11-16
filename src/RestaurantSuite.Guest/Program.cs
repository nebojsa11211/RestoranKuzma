using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using RestaurantSuite.Guest;
using RestaurantSuite.Guest.Services;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient for API communication
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl") ?? "http://localhost:5213";

// Register LocalStorage service
builder.Services.AddBlazoredLocalStorage();

// Register Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Register HttpClient and Services
// Register the authentication message handler
builder.Services.AddScoped<AuthenticationMessageHandler>();

// Register HttpClient with authentication handler
builder.Services.AddScoped(sp =>
{
    var authHandler = sp.GetRequiredService<AuthenticationMessageHandler>();
    authHandler.InnerHandler = new HttpClientHandler();

    var httpClient = new HttpClient(authHandler)
    {
        BaseAddress = new Uri(apiBaseUrl)
    };

    return httpClient;
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MenuApiService>();
builder.Services.AddScoped<CategoriesApiService>();
builder.Services.AddScoped<OrdersApiService>();
builder.Services.AddScoped<TablesApiService>();
builder.Services.AddScoped<MobileInteractionService>();
builder.Services.AddScoped<ChatApiService>();
builder.Services.AddScoped<ChatHubService>();
builder.Services.AddScoped<DailyMenuApiService>();
builder.Services.AddScoped<OrderSessionApiService>();
builder.Services.AddScoped<OrdersHubService>();
builder.Services.AddScoped<RestaurantSettingsService>();
builder.Services.AddScoped<CacheService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<LocalizationService>();

// Register Authentication Services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<RestaurantSuite.Shared.Blazor.Services.Auth.ITokenService, RestaurantSuite.Shared.Blazor.Services.Auth.TokenService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthenticationStateProvider>());

await builder.Build().RunAsync();
