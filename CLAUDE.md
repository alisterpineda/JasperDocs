# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 9.0 Aspire application with an Angular 20 frontend. The solution follows a vertical slice architecture pattern for the backend and uses ASP.NET Core Identity with PostgreSQL for user management.

## Solution Structure

- **Index.AppHost**: .NET Aspire orchestration host that manages service dependencies and infrastructure (PostgreSQL with PgWeb)
- **Index.WebApi**: ASP.NET Core Web API backend (.NET 9.0)
- **Index.WebApp**: Angular 20 frontend application
- **Index.ServiceDefaults**: Shared Aspire service configurations (OpenTelemetry, health checks, service discovery)

## Build and Run Commands

### Running the entire application
```bash
dotnet run --project Index.AppHost/Index.AppHost.csproj
```
This starts the Aspire AppHost which orchestrates all services including PostgreSQL and the Web API.

### Backend (WebApi) only
```bash
dotnet build Index.WebApi/Index.WebApi.csproj
dotnet run --project Index.WebApi/Index.WebApi.csproj
```

### Frontend (WebApp) only
```bash
cd Index.WebApp
npm install        # First time only
npm start          # Runs ng serve on 127.0.0.1:4200
npm run build      # Production build
npm test           # Run Karma/Jasmine tests
```

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add <MigrationName> --project Index.WebApi

# Apply migrations (also auto-applied on app startup in Program.cs)
dotnet ef database update --project Index.WebApi
```

### Testing
```bash
# Frontend tests
cd Index.WebApp
npm test
```

## Architecture Patterns

### Backend: Vertical Slice Architecture

The WebApi project uses a vertical slice architecture where features are organized by domain concept rather than technical layer:

```
Features/
  Documents/
    DocumentsController.cs     # API endpoint
    CreateDocument.cs          # Request DTO
    CreateDocumentHandler.cs   # Request handler
```

Each feature contains its:
- Controller (API endpoints)
- Request/Response DTOs
- Request handlers implementing `IRequestHandler<TRequest>` or `IRequestHandler<TRequest, TResponse>`

Register handlers in `Program.cs`:
```csharp
builder.Services.AddScoped<IRequestHandler<CreateDocument>, CreateDocumentHandler>();
```

### Entity Configuration

Entity configurations are separate from entities and auto-discovered:
- Entity classes in `Entities/`
- Configuration classes in `Infrastructure/EntityConfigurations/` implementing `IEntityTypeConfiguration<T>`
- Configurations are auto-applied via `modelBuilder.ApplyConfigurationsFromAssembly()` in `ApplicationDbContext.OnModelCreating()`

### Identity and Authentication

- Uses ASP.NET Core Identity with `ApplicationUser : IdentityUser<Guid>`
- Identity tables use `Guid` as primary key type
- Identity API endpoints mapped via `app.MapIdentityApi<ApplicationUser>()`
- All controllers require authorization by default: `app.MapControllers().RequireAuthorization()`

### Frontend Integration

The WebApi serves the Angular build output:
- Angular build artifacts placed in `wwwroot/browser`
- Static files served from root path
- Fallback routing to `browser/index.html` for Angular routing

### Database

- PostgreSQL via Aspire's Npgsql provider
- Connection string configured in AppHost: `builder.AddNpgsqlDbContext<ApplicationDbContext>(connectionName: "postgresdb")`
- Migrations auto-applied on startup (see TODO in Program.cs about safer migration approach)
- PgWeb included for database management UI during development

## Development Notes

### Adding a New Feature

1. Create feature folder under `Features/`
2. Add request/response DTOs
3. Create handler implementing `IRequestHandler<>`
4. Add controller with endpoints
5. Register handler in `Program.cs`

### Adding a New Entity

1. Create entity class in `Entities/`
2. Add `DbSet<TEntity>` property to `ApplicationDbContext`
3. Create configuration class in `Infrastructure/EntityConfigurations/` (if needed for custom mappings)
4. Run `dotnet ef migrations add <MigrationName> --project Index.WebApi`

### Aspire Service Defaults

The `Index.ServiceDefaults` project provides common Aspire infrastructure:
- OpenTelemetry (metrics, tracing, logging)
- Health checks at `/health` and `/alive` (development only)
- Service discovery
- HTTP client resilience handlers

Use via `builder.AddServiceDefaults()` in service startup.

### API Documentation

In development mode:
- OpenAPI endpoint mapped via `app.MapOpenApi()`
- Scalar UI available via `app.MapScalarApiReference()`
