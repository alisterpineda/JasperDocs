var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Index_WebApi>("index-webapi");

builder.Build().Run();
