var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithPgWeb();
var postgresDb = postgres.AddDatabase("AppDatabase", "jasper-docs");

builder.AddProject<Projects.JasperDocs_WebApi>("jasperdocs-webapi")
    .WithReference(postgresDb)
    .WaitFor(postgresDb);

builder.Build().Run();
