using System;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RestaurantSuite.Infrastructure.EF;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Infrastructure.EF.Repositories;
using RestaurantSuite.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Helper: Trim and remove surrounding quotes
static string StripQuotes(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim().Trim('\"', '\'');

// Helper: Resolve connection string from multiple possible locations (configuration, env vars, common keys)
static string ResolveConnectionString(Microsoft.Extensions.Configuration.IConfiguration config)
{
    // 1) Named connection string from configuration
    var conn = config.GetConnectionString("DefaultConnection");
    conn = StripQuotes(conn);
    if (!string.IsNullOrWhiteSpace(conn))
        return conn;

    // 2) Direct configuration key
    conn = StripQuotes(config["ConnectionStrings:DefaultConnection"]);
    if (!string.IsNullOrWhiteSpace(conn))
        return conn;

    // 3) Common environment variable names (check environment first)
    string[] envNames = new[] { "DefaultConnection", "CONNECTION_STRING", "CONN_STRING", "DATABASE_URL", "DB_CONNECTION" };
    foreach (var name in envNames)
    {
        var env = StripQuotes(Environment.GetEnvironmentVariable(name));
        if (!string.IsNullOrWhiteSpace(env))
            return env;
    }

    // 4) Then check same keys in configuration (sometimes set by hosting providers)
    foreach (var name in envNames)
    {
        var cfg = StripQuotes(config[name]);
        if (!string.IsNullOrWhiteSpace(cfg))
            return cfg;
    }

    // 5) Nothing found
    return null;
}

// Add services to the container.

// Database - Environment-based selection
// Development: SQLite (no IPv6 issues, fast local dev)
// Production: Supabase PostgreSQL (when deployed to cloud with IPv6)

// 1) Try named connection string from configuration or environment-friendly fallbacks
var connectionString = ResolveConnectionString(builder.Configuration);

// 2) Determine database provider from config (allow override later if we detect DATABASE_URL)
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";

// 3) If connectionString looks like a DATABASE_URL (postgres://...), convert it and set provider
if (!string.IsNullOrWhiteSpace(connectionString))
{
    var trimmed = connectionString.Trim();
    if (trimmed.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
        trimmed.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    {
        connectionString = ConvertPostgresUrlToConnectionString(trimmed);
        databaseProvider = "PostgreSQL";
    }
}

// 4) If still missing, provide sensible defaults or throw if required
if (string.IsNullOrWhiteSpace(connectionString))
{
    if (databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException("Database connection string for PostgreSQL is not configured. Please set 'ConnectionStrings:DefaultConnection' or the 'DATABASE_URL' environment variable.");
    }
    else
    {
        // Default to a local sqlite file for development convenience
        connectionString = "Data Source=D:\\KimiTest\\RestoranKuzma\\src\\RestaurantSuite.Api\\restorankuzma.db";
    }
}

// Register DbContext using the resolved connection string and provider
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
builder.Services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IOrderSessionRepository, OrderSessionRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IDailyMenuRepository, DailyMenuRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Authentication Service
builder.Services.AddScoped<IAuthenticationService, JwtAuthenticationService>();

// Invoice Service
builder.Services.AddScoped<IInvoiceService, RestaurantSuite.Infrastructure.EF.Services.InvoiceService>();

// Database Browser Service (Admin tool)
builder.Services.AddScoped<IDatabaseBrowserService, RestaurantSuite.Infrastructure.EF.Services.DatabaseBrowserService>();

// Mock notification service for now
builder.Services.AddScoped<INotificationService, MockNotificationService>();

// Memory cache for database browser metadata caching
builder.Services.AddMemoryCache();

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
        ClockSkew = TimeSpan.Zero, // Remove default 5 minute clock skew
        NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };

    // Configure SignalR JWT authentication
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // If the request is for our SignalR hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                // Read the token out of the query string
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
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

// SignalR for real-time communication
builder.Services.AddSignalR();

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

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
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

// Map SignalR Hub
app.MapHub<RestaurantSuite.Api.Hubs.ChatHub>("/hubs/chat");

app.Run();

static string ConvertPostgresUrlToConnectionString(string databaseUrl)
{
    // Examples of databaseUrl: postgres://username:password@host:5432/dbname
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);
    var username = userInfo.Length > 0 ? WebUtility.UrlDecode(userInfo[0]) : "";
    var password = userInfo.Length > 1 ? WebUtility.UrlDecode(userInfo[1]) : "";
    var database = uri.AbsolutePath?.TrimStart('/') ?? "";

    // Build connection string for Npgsql
    // Include SSL and Trust Server Certificate to work with many hosted providers
    var portPart = uri.Port > 0 ? uri.Port : 5432;
    return $"Host={uri.Host};Port={portPart};Username={username};Password={password};Database={database};SSL Mode=Require;Trust Server Certificate=true";
}
