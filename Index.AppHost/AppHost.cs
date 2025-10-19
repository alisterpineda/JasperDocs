var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithPgWeb();
var postgresDb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.Index_WebApi>("index-webapi")
    .WithReference(postgresDb);

builder.Build().Run();
