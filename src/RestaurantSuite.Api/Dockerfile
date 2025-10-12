FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/RestaurantSuite.Api/RestaurantSuite.Api.csproj", "src/RestaurantSuite.Api/"]
COPY ["src/RestaurantSuite.Application/RestaurantSuite.Application.csproj", "src/RestaurantSuite.Application/"]
COPY ["src/RestaurantSuite.Domain/RestaurantSuite.Domain.csproj", "src/RestaurantSuite.Domain/"]
COPY ["src/RestaurantSuite.Infrastructure.EF/RestaurantSuite.Infrastructure.EF.csproj", "src/RestaurantSuite.Infrastructure.EF/"]
COPY ["src/RestaurantSuite.Infrastructure.Identity/RestaurantSuite.Infrastructure.Identity.csproj", "src/RestaurantSuite.Infrastructure.Identity/"]
COPY ["src/RestaurantSuite.Notifications/RestaurantSuite.Notifications.csproj", "src/RestaurantSuite.Notifications/"]
COPY ["src/RestaurantSuite.Integrations/RestaurantSuite.Integrations.csproj", "src/RestaurantSuite.Integrations/"]
COPY ["src/RestaurantSuite.Common/RestaurantSuite.Common.csproj", "src/RestaurantSuite.Common/"]
RUN dotnet restore "src/RestaurantSuite.Api/RestaurantSuite.Api.csproj"
COPY . .
WORKDIR "/src/src/RestaurantSuite.Api"
RUN dotnet build "RestaurantSuite.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RestaurantSuite.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RestaurantSuite.Api.dll"]
