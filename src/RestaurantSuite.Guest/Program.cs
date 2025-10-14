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
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Register LocalStorage service
builder.Services.AddBlazoredLocalStorage();

// Register API service
builder.Services.AddScoped<ApiService>();

// Register Authentication services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthenticationStateProvider>());

// Register Mobile Interaction service
builder.Services.AddScoped<MobileInteractionService>();

// Add authorization services
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
