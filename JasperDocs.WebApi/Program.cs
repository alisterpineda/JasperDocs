using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Entities;
using JasperDocs.WebApi.Features.Documents;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddNpgsqlDbContext<ApplicationDbContext>(connectionName: "AppDatabase");
builder.Services.AddScoped<IRequestHandler<CreateDocument>, CreateDocumentHandler>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add CORS for Vite dev server in development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ViteDevServer", policy =>
        {
            policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });
}

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Use CORS for Vite dev server in development
if (app.Environment.IsDevelopment())
{
    app.UseCors("ViteDevServer");
}

app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.MapIdentityApi<ApplicationUser>();

// Serve React app from wwwroot (production only)
if (!app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.MapFallbackToFile("index.html");
}


// TODO: Consider a different, safer migration approach later
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();
