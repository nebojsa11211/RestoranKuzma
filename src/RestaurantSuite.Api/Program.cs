using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RestaurantSuite.Infrastructure.EF;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Infrastructure.EF.Repositories;
using RestaurantSuite.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database - Environment-based selection
// Development: SQLite (no IPv6 issues, fast local dev)
// Production: Supabase PostgreSQL (when deployed to cloud with IPv6)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "Sqlite"; // Default to SQLite

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RestaurantSuite.Application.Commands.CreateRestaurantCommand).Assembly));

// Repositories
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<IMenuItemIngredientRepository, MenuItemIngredientRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IStockTransactionRepository, StockTransactionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Authentication Service
builder.Services.AddScoped<IAuthenticationService, JwtAuthenticationService>();

// Invoice Service
builder.Services.AddScoped<IInvoiceService, RestaurantSuite.Infrastructure.EF.Services.InvoiceService>();

// Mock notification service for now
builder.Services.AddScoped<INotificationService, MockNotificationService>();

// JWT Authentication Configuration
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "development-secret-key-minimum-32-characters-long-for-security";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "RestaurantSuite";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "RestaurantSuite";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero // Remove default 5 minute clock skew
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Restaurant Suite API",
        Version = "v1",
        Description = "API for Restaurant Management System"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant Suite API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Restaurant Suite API Documentation";
    });
}

// Disable HTTPS redirection in development to allow HTTP communication
// app.UseHttpsRedirection();

app.UseCors();

// Authentication & Authorization middleware - order matters!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
