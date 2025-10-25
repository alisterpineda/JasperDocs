using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// External parameter for document storage path
var dataDirPath = builder.AddParameter("data-dir-path");

// External parameters for initial admin user creation
var initialAdminUsername = builder.AddParameter("initial-admin-username");
var initialAdminPassword = builder.AddParameter("initial-admin-password", secret: true);

var postgres = builder.AddPostgres("postgres").WithPgWeb();
var postgresDb = postgres.AddDatabase("AppDatabase", "jasper-docs");

var webApi = builder.AddProject<Projects.JasperDocs_WebApi>("jasperdocs-webapi")
    .WithReference(postgresDb)
    .WithEnvironment("DATA_DIR_PATH", dataDirPath)
    .WithEnvironment("INITIAL_ADMIN_USERNAME", initialAdminUsername)
    .WithEnvironment("INITIAL_ADMIN_PASSWORD", initialAdminPassword)
    .WaitFor(postgresDb);

// In development, run Vite dev server for HMR
if (builder.Environment.IsDevelopment())
{
    builder.AddNpmApp("jasperdocs-webapp", "../JasperDocs.WebApp", "dev")
        .WithHttpEndpoint(port: 5173, isProxied: false)
        .WithExternalHttpEndpoints()
        .WithReference(webApi)
        .WithEnvironment("VITE_API_URL", webApi.GetEndpoint("https"))
        .WaitFor(webApi);
}

builder.Build().Run();
