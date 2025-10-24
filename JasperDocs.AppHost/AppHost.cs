using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithPgWeb();
var postgresDb = postgres.AddDatabase("AppDatabase", "jasper-docs");

var webApi = builder.AddProject<Projects.JasperDocs_WebApi>("jasperdocs-webapi")
    .WithReference(postgresDb)
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
