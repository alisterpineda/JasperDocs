var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Index_Api>("index-api");

builder.Build().Run();
