# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 9.0 Aspire application with a React 19 frontend. The solution follows a vertical slice architecture pattern for the backend and uses ASP.NET Core Identity with PostgreSQL for user management.

## Solution Structure

- **JasperDocs.AppHost**: .NET Aspire orchestration host that manages service dependencies and infrastructure (PostgreSQL with PgWeb)
- **JasperDocs.WebApi**: ASP.NET Core Web API backend (.NET 9.0)
- **JasperDocs.WebApp**: React 19 frontend application (Vite + TypeScript)
- **JasperDocs.ServiceDefaults**: Shared Aspire service configurations (OpenTelemetry, health checks, service discovery)

## Build and Run Commands

### Running the entire application
```bash
dotnet run --project JasperDocs.AppHost/JasperDocs.AppHost.csproj
```
This starts the Aspire AppHost which orchestrates all services:
- **Development mode**: Starts PostgreSQL, WebApi, and Vite dev server (port 5173) with HMR
- **Production mode**: Starts PostgreSQL and WebApi only (React served as static files from wwwroot)

### Backend (WebApi) only
```bash
dotnet build JasperDocs.WebApi/JasperDocs.WebApi.csproj
dotnet run --project JasperDocs.WebApi/JasperDocs.WebApi.csproj
```

### Frontend (WebApp) only
```bash
cd JasperDocs.WebApp
npm install        # First time only
npm run dev        # Runs Vite dev server on localhost:5173
npm run build      # Production build (outputs to dist/)
npm run preview    # Preview production build
```

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add <MigrationName> --project JasperDocs.WebApi

# Apply migrations (also auto-applied on app startup in Program.cs)
dotnet ef database update --project JasperDocs.WebApi
```

### Testing
```bash
# Frontend linting
cd JasperDocs.WebApp
npm run lint
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

**Important**:
- Handlers return domain types and throw exceptions for error cases (decoupled from ASP.NET)
- Handlers throw `NotFoundException` (404), `ValidationException` (400), or other domain exceptions
- Global `ExceptionHandlingMiddleware` converts domain exceptions to HTTP responses automatically
- Controllers convert domain types to HTTP responses when needed (e.g., `FileDownloadInfo` → `PhysicalFile()`)
- All API endpoints are prefixed with `/api` via `RoutePrefixConvention` (e.g., `/api/login`, `/api/documents`)
- **Exception**: `RefreshHandler` returns `IResult` due to deep integration with Identity framework's bearer token mechanism

Register handlers in `Program.cs`:
```csharp
builder.Services.AddScoped<IRequestHandler<CreateDocument>, CreateDocumentHandler>();
```

### Domain Model: Documents and Versioning

**Document** is an aggregate root with **DocumentVersion** child entities:
- Document: Title (filename without extension), Description (nullable), CreatedByUserId, Versions collection
- DocumentVersion: DocumentId, VersionNumber, Description, **StoragePath**, MimeType, **OriginalFileName**, **FileExtension** (nullable), CreatedByUserId, CreatedAt
- Unique constraint: (DocumentId, VersionNumber)
- Cascade delete: Deleting Document removes all Versions

**File Upload & Storage**:
- Files stored with GUID-based names: `{DataDirectoryPath}/documents/{documentId}/{versionId}{.ext}`
- StoragePath contains relative path from DataDirectoryPath (e.g., `documents/abc/xyz.pdf`)
- OriginalFileName preserves full uploaded filename for audit trail
- FileExtension extracted via `FileExtensionHelper` (supports compound extensions: `.tar.gz`, `.tar.bz2`, `.tar.xz`, `.tar.zst`)
- Document.Title set to filename without extension (e.g., `report.tar.gz` → Title: `report`)
- Downloads reconstruct filename as `Title + FileExtension` for user-friendly names

**Versioning Behavior**:
- Creating a Document requires file upload via multipart/form-data
- Auto-creates Version 1 with uploaded file (see `CreateDocumentHandler`)
- Version numbers auto-increment (finds max + 1)
- Create new version: POST `/api/documents/versions` with file upload

**Endpoints**:
- `GET /api/documents` - List documents with pagination (pageNumber, pageSize query params)
- `GET /api/documents/{id}?versionNumber={optional}` - Get document with specific or latest version
- `POST /api/documents` - Create document with file upload
- `POST /api/documents/versions` - Create new version with file upload
- `GET /api/documents/versions/{versionId}/file` - Download/stream document file (requires auth, returns file with proper MIME type)

**DDD Pattern**: All Document-related endpoints under `DocumentsController` since Document is the aggregate root.

**File Access Security**: File download endpoint validates path traversal, ensures files are within DataDirectoryPath, and returns 404 if version/file not found.

### Request/Response Flow

**Handler Interfaces** (`Core/IRequestHandler.cs`):
```csharp
IRequestHandler<TRequest, TResponse>  // Returns Task<TResponse>
IRequestHandler<TRequest>             // Returns Task (no response data)
```

**Error Handling** (`Core/Exceptions/`):
- `DomainException` - Base class for all domain exceptions
- `NotFoundException` - Thrown when resource not found (maps to 404)
- `ValidationException` - Thrown when validation fails (maps to 400)
- Global `ExceptionHandlingMiddleware` catches exceptions and returns ProblemDetails

**File Downloads** (`Core/FileDownloadInfo.cs`):
- Handlers return `FileDownloadInfo` domain model (FilePath, MimeType, FileName)
- Controllers convert to HTTP file results using `PhysicalFile()` method

**Controller Pattern** - Inject handlers per endpoint via `[FromServices]`:
```csharp
// JSON body
[HttpPost]
public Task CreateAsync(
    [FromServices] IRequestHandler<CreateDocument> handler,
    [FromBody] CreateDocument request,
    CancellationToken ct = default)
{
    return handler.HandleAsync(request, ct);
}

// File upload (multipart/form-data)
[HttpPost]
[Consumes("multipart/form-data")]
public Task UploadAsync(
    [FromServices] IRequestHandler<CreateDocument> handler,
    [FromForm] CreateDocument request,
    CancellationToken ct = default)
{
    return handler.HandleAsync(request, ct);
}
```

**Authentication Helpers** (`Core/HttpContextAccessorExtensions.cs`):
- `httpContextAccessor.GetUserId()` → `Guid?` from ClaimTypes.NameIdentifier
- `httpContextAccessor.IsAuthenticated()` → `bool`

**Example Flow**:
1. Request DTO: `CreateDocument.cs` (POCO with required/optional properties)
2. Handler: `CreateDocumentHandler : IRequestHandler<CreateDocument>` (constructor injection for dependencies)
   - Returns domain types or void
   - Throws `NotFoundException`, `ValidationException`, or other domain exceptions for errors
3. Controller: Inject handler via `[FromServices]`, deserialize body via `[FromBody]`
   - Converts domain types to HTTP responses (e.g., `FileDownloadInfo` → `PhysicalFile()`)
   - Exception middleware automatically handles exceptions → ProblemDetails responses
4. Registration: `AddScoped<IRequestHandler<CreateDocument>, CreateDocumentHandler>()`

**Pagination Pattern** (`Core/PaginatedResponse.cs`):
```csharp
public class PaginatedResponse<T>
{
    public required IReadOnlyList<T> Data { get; init; }
    public required int TotalCount { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
```
- Handler returns `PaginatedResponse<TDto>` with data and metadata
- Controller accepts `pageNumber` and `pageSize` as `[FromQuery]` parameters
- Example: `ListDocumentsHandler` sorts by `UpdatedAt DESC`, caps pageSize at 100

### Entity Configuration

Entity configurations are separate from entities and auto-discovered:
- Entity classes in `Entities/`
- Configuration classes in `Infrastructure/EntityConfigurations/` implementing `IEntityTypeConfiguration<T>`
- Naming convention: `{EntityName}EntityTypeConfiguration.cs` (e.g., `DocumentEntityTypeConfiguration.cs`)
- Configurations are auto-applied via `modelBuilder.ApplyConfigurationsFromAssembly()` in `ApplicationDbContext.OnModelCreating()`

### Identity and Authentication

- Uses ASP.NET Core Identity with `ApplicationUser : IdentityUser<Guid>` (Guid primary keys)
- Configured with `AddIdentityApiEndpoints<ApplicationUser>()` for bearer token support
- Custom authentication endpoints in `Features/Authentication/` (vertical slice pattern):
  - `POST /api/login` - Accepts `username` and `password`, returns `AccessTokenResponse` (access token, refresh token, expiresIn)
  - `POST /api/refresh` - Refreshes tokens using refresh token
  - `POST /api/logout` - Signs out user (requires auth)
- All controllers require authorization by default: `app.MapControllers().RequireAuthorization()`
- Bearer tokens returned via `SignInManager` with `IdentityConstants.BearerScheme`
- Tokens are ASP.NET Core Identity proprietary format (not JWTs)

**Admin User**: Database seeder creates admin user on first startup if configured (`DatabaseSeeder.cs`):
- Requires `INITIAL_ADMIN_USERNAME` and `INITIAL_ADMIN_PASSWORD` environment variables
- Only runs when database has zero users
- Created user has `Email: null`, `EmailConfirmed: false`
- Configure via AppHost external parameters: `initial-admin-username` and `initial-admin-password` (secret)

### Configuration: Options Pattern

**StorageOptions** (`Core/StorageOptions.cs`): Configures file storage location
- `DataDirectoryPath`: Root directory for document uploads (must be absolute path)
- Configured via `DATA_DIR_PATH` environment variable
- Mapped to `Storage:DataDirectoryPath` config key using in-memory provider
- Injected via `IOptionsMonitor<StorageOptions>` for dynamic configuration updates
- **Validation**: Handlers validate path is non-empty and absolute before use

**Usage**:
```csharp
public class MyHandler(IOptionsMonitor<StorageOptions> storageOptions)
{
    var path = storageOptions.CurrentValue.DataDirectoryPath;
    if (string.IsNullOrWhiteSpace(path))
        throw new InvalidOperationException("DataDirectoryPath is not configured.");
    if (!Path.IsPathFullyQualified(path))
        throw new InvalidOperationException($"DataDirectoryPath must be absolute. Current: {path}");
}
```

### Frontend Architecture

**UI Framework**: Mantine v7 with Tabler Icons
- All components wrapped in `MantineProvider` (`main.tsx`)
- CSS imported via `@mantine/core/styles.css`

**Layout Pattern**: Mantine AppShell (`components/Layout/AppLayout.tsx`)
- Navbar (60px height) with branding + auth UI
- Collapsible sidebar (250px, collapses on mobile)
- Login page bypasses layout shell

**Authentication**: Context-based (`contexts/AuthContext.tsx`, `hooks/useAuth.ts`)
- State: `isAuthenticated`, `user`, `login(tokenResponse, username)`, `logout()`, `refreshTokens()`
- Token storage: `localStorage.authToken` (access), `localStorage.refreshToken` (refresh), `localStorage.username`
- Automatic token refresh: 5 minutes before expiry via `services/tokenRefresh.ts`
- Multi-tab sync: Storage events keep auth state synchronized across tabs
- Axios interceptors: Auto-refresh on 401, retry failed requests with new token
- Protected routes redirect to `/login` if unauthenticated

**Routing**: TanStack Router with file-based routing
- `/` - Home page
- `/documents` - Protected documents list (index route)
- `/documents/{id}` - Document detail view with PDF preview
- `/login` - Authentication page

**Document Preview** (`pages/DocumentDetail.tsx`):
- PDFs: Fetches file via `AXIOS_INSTANCE` (includes auth token), creates blob URL for `<iframe>` preview
- Other files: Shows "Preview not supported" message with MIME type
- Blob URLs cleaned up on unmount to prevent memory leaks

**Nested Routes Pattern**: Parent routes with children must render `<Outlet />`:
```
routes/
  documents.tsx          # Parent: renders <Outlet /> for children
  documents/
    index.tsx           # Child: /documents (list view)
    $documentId.tsx     # Child: /documents/{id} (detail view)
```

**Structure**:
```
src/
  components/Layout/  # AppLayout, Navbar, Sidebar
  pages/              # Home, Documents, Login
  contexts/           # AuthContext (provider), auth-context (context definition)
  hooks/              # useAuth
  services/           # tokenRefresh (singleton for token management)
  api/                # Generated clients, axios config
```

**Development mode** (default when running AppHost):
- Vite dev server runs separately via Aspire on port 5173 with HMR
- Vite proxy forwards `/api/*` requests to WebApi (configured in vite.config.ts)
- Aspire injects `VITE_API_URL` env var with WebApi endpoint
- CORS enabled on WebApi for localhost:5173
- Static file serving disabled in WebApi

**Production/Release mode**:
- MSBuild target auto-builds React before WebApi build (Release configuration only)
- Build output goes to `wwwroot/` (configured in vite.config.ts)
- WebApi serves static files from `wwwroot/`
- Fallback routing to `index.html` for React Router

### API Client (Frontend)

Uses **Orval** to generate type-safe TanStack Query hooks from OpenAPI. Axios handles HTTP with auth interceptors.

**Regenerate after API changes**:
```bash
dotnet build JasperDocs.WebApi/JasperDocs.WebApi.csproj
cd JasperDocs.WebApp && npm run api:generate
# Commit generated code in src/api/generated/
```

**Usage**:
```typescript
// Generated hooks: usePostApiLogin, useGetApiDocuments, usePostApiDocuments, etc.
import { usePostApiDocuments, useGetApiDocuments } from './api/generated/documents/documents';
import { usePostApiLogin } from './api/generated/authentication/authentication';

const loginMutation = usePostApiLogin();
const response = await loginMutation.mutateAsync({ data: { email, password } });
// Response: { accessToken, refreshToken, tokenType, expiresIn }

// List with pagination
const { data } = useGetApiDocuments({ pageNumber: 1, pageSize: 25 });
// data = PaginatedResponse<DocumentListItemDto> directly (see note below)

const docMutation = usePostApiDocuments();
await docMutation.mutateAsync({ data: { File: file } }); // For file uploads
```

**IMPORTANT - Axios Response Unwrapping**:
`customAxiosInstance` automatically unwraps axios responses (`.then(({ data }) => data)`). The `data` returned from hooks is the API response object directly, NOT wrapped in `{ data: ... }`.

Example: `useGetApiDocuments` returns `PaginatedResponse<DocumentListItemDto>`, access as:
- `data.data` (documents array)
- `data.totalCount`, `data.pageNumber`, etc.

**Auth**: Tokens auto-injected via axios interceptor. Automatic refresh on 401 errors.
**Config**: `orval.config.ts`, `src/main.tsx` (QueryClientProvider, 5min cache)

**Direct axios usage**: For file downloads or custom requests, import `AXIOS_INSTANCE` from `api/axios-instance.ts` (includes auth interceptors, useful for blob responses)

### Database

- PostgreSQL via Aspire's Npgsql provider
- Connection string configured in AppHost: `builder.AddNpgsqlDbContext<ApplicationDbContext>(connectionName: "AppDatabase")`
- Migrations auto-applied on startup (see TODO in Program.cs about safer migration approach)
- PgWeb included for database management UI during development

**Transactions with Npgsql**: Use execution strategy wrapper for manual transactions:
```csharp
var strategy = _context.Database.CreateExecutionStrategy();
await strategy.ExecuteAsync(async () =>
{
    await using var transaction = await _context.Database.BeginTransactionAsync(ct);
    try {
        // DB operations and file I/O
        await transaction.CommitAsync(ct);
    }
    catch { throw; } // Auto-rollback on dispose
});
```

## Development Notes

### Adding a New Feature

1. Create feature folder under `Features/`
2. Add request/response DTOs
3. Create handler implementing `IRequestHandler<>` (return domain types, not HTTP types)
4. Add controller with endpoints (convert domain results to HTTP responses)
5. Add `[ProducesResponseType<T>]` attributes for OpenAPI documentation
6. Register handler in `Program.cs`
7. Rebuild WebApi and regenerate API client: `dotnet build JasperDocs.WebApi/JasperDocs.WebApi.csproj && cd JasperDocs.WebApp && npm run api:generate`

**Note**: All endpoints automatically prefixed with `/api` via `RoutePrefixConvention`. Controllers don't need to include `/api` in their routes.

### Adding a New Entity

1. Create entity class in `Entities/`
2. Add `DbSet<TEntity>` property to `ApplicationDbContext`
3. Create configuration class in `Infrastructure/EntityConfigurations/` (if needed for custom mappings)
4. Run `dotnet ef migrations add <MigrationName> --project JasperDocs.WebApi`

### Aspire Service Discovery and Integration

**AppHost Configuration** (`JasperDocs.AppHost/AppHost.cs`):
- External parameters: `data-dir-path`, `initial-admin-username`, `initial-admin-password` (secret)
- WebApp references WebApi via `.WithReference(webApi)`
- Aspire injects `VITE_API_URL` with **HTTPS** endpoint: `webApi.GetEndpoint("https")`
- WebApp waits for WebApi to start via `.WaitFor(webApi)`

**Vite Proxy** (`JasperDocs.WebApp/vite.config.ts`):
```typescript
proxy: {
  '/api': {
    target: process.env.VITE_API_URL || 'http://localhost:5000',
    changeOrigin: true,
    secure: false,
  }
}
```
- Development: Proxies `/api/*` to WebApi service URL from Aspire
- Production: Not used (same-origin requests to WebApi serving static files)
- Standalone: Falls back to `localhost:5000` when `VITE_API_URL` not set

**Service Defaults** (`JasperDocs.ServiceDefaults`):
- OpenTelemetry (metrics, tracing, logging)
- Health checks at `/health` and `/alive` (development only)
- Service discovery
- HTTP client resilience handlers

Use via `builder.AddServiceDefaults()` in service startup.

### API Documentation

In development mode:
- OpenAPI endpoint mapped via `app.MapOpenApi()`
- Scalar UI available via `app.MapScalarApiReference()`
