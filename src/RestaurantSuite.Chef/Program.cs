using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RestaurantSuite.Chef;
using RestaurantSuite.Chef.Services;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient for API communication
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl") ?? "http://localhost:5213";

// Register LocalStorage service
builder.Services.AddBlazoredLocalStorage();

// Register HttpClient and Services
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<OrdersApiService>();
builder.Services.AddScoped<MenuApiService>();
builder.Services.AddScoped<CategoriesApiService>();

await builder.Build().RunAsync();
