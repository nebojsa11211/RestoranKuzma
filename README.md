# Restoran Kuzma Management System

A comprehensive restaurant management system built with .NET 9, Blazor, and PostgreSQL for Restoran Kuzma. The system supports multiple user roles (Admin, Waiter, Chef, Guest) with real-time order tracking via SignalR.

## Features

- **Role-based access control** (Admin, Waiter, Chef, Guest)
- **Real-time order tracking** with SignalR notifications
- **Progressive Web Apps** (PWA) for Waiter, Chef, and Guest frontends
- **Payment integration** with Stripe and Square adapters
- **Printer integration** for ESC/POS printers
- **PostgreSQL database** for data persistence
- **Redis caching** and SignalR backplane
- **JWT authentication** with ASP.NET Core Identity
- **Menu management** with categories and items
- **Order management** with status tracking
- **Kitchen display system** for real-time order preparation
- **Table management** for dine-in service
- **Comprehensive testing** (Unit, Integration, E2E)
- **Docker containerization** with docker-compose
- **CI/CD pipeline** with GitHub Actions

## Architecture

The solution follows Clean Architecture principles:

```
├── src/
│   ├── RestaurantSuite.Domain/          # Core domain entities
│   ├── RestaurantSuite.Application/     # Business logic, CQRS
│   ├── RestaurantSuite.Infrastructure.EF/    # Data access
│   ├── RestaurantSuite.Infrastructure.Identity/  # Authentication
│   ├── RestaurantSuite.Notifications/   # SignalR hubs
│   ├── RestaurantSuite.Integrations/    # Payment & printer adapters
│   ├── RestaurantSuite.Common/          # Shared utilities
│   ├── RestaurantSuite.Api/             # REST API
│   ├── RestaurantSuite.Admin/           # Blazor Server admin dashboard
│   ├── RestaurantSuite.Waiter/          # Blazor WASM PWA
│   ├── RestaurantSuite.Chef/            # Blazor WASM PWA
│   └── RestaurantSuite.Guest/           # Blazor WASM PWA
├── tests/
│   ├── RestaurantSuite.Tests.Unit/      # xUnit unit tests
│   └── RestaurantSuite.Tests.Integration/  # Integration tests
└── specs/                                # Specification documents
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [PostgreSQL 16](https://www.postgresql.org/download/) (or use Docker)
- [Redis](https://redis.io/download) (or use Docker)

## Getting Started

### 1. Clone the repository

```bash
git clone <repository-url>
cd RestoranKuzma
```

### 2. Run with Docker Compose

```bash
docker-compose up -d
```

The API will be available at `http://localhost:8080`

### 3. Run locally

```bash
# Restore dependencies
dotnet restore

# Update database
cd src/RestaurantSuite.Api
dotnet ef database update

# Run the API
dotnet run --project src/RestaurantSuite.Api

# Run the Admin dashboard
dotnet run --project src/RestaurantSuite.Admin
```

## Configuration

Update `appsettings.json` or use environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=restaurantsuite;Username=postgres;Password=postgres"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  },
  "JWT": {
    "Secret": "your-secret-key",
    "Issuer": "RestaurantSuite",
    "Audience": "RestaurantSuite"
  },
  "Stripe": {
    "SecretKey": "your-stripe-secret-key"
  },
  "Square": {
    "AccessToken": "your-square-access-token",
    "LocationId": "your-location-id"
  }
}
```

## Testing

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test tests/RestaurantSuite.Tests.Unit

# Run integration tests only
dotnet test tests/RestaurantSuite.Tests.Integration
```

## Development Workflow

This project follows Test-Driven Development (TDD):

1. Write tests first (Phase 2)
2. Implement domain entities (Phase 3)
3. Implement application layer (Phase 4)
4. Implement infrastructure (Phase 5)
5. Build UI components (Phase 6)

## Tech Stack

- **.NET 9** - Framework
- **Blazor Server & WASM** - UI frameworks
- **ASP.NET Core** - REST API
- **Entity Framework Core 9** - ORM
- **PostgreSQL 16** - Primary database
- **Redis 7** - Caching & SignalR backplane
- **SignalR** - Real-time communication
- **MediatR** - CQRS pattern
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping
- **xUnit** - Testing framework
- **Testcontainers** - Integration testing
- **Docker** - Containerization
- **GitHub Actions** - CI/CD

## Contributing

1. Fork the repository
2. Create a feature branch
3. Write tests for your changes
4. Implement the feature
5. Ensure all tests pass
6. Submit a pull request

## License

This project is licensed under the MIT License.

## Contact

For questions or support, please open an issue in the repository.
