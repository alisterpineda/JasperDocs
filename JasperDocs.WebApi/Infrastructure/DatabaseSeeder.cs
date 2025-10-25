using JasperDocs.WebApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services, string username, string password)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(DatabaseSeeder));

        // Only seed if there are zero users in the database
        if (await userManager.Users.AnyAsync())
        {
            return;
        }

        // Create admin user
        var adminUser = new ApplicationUser
        {
            Email = null,
            UserName = username,
            EmailConfirmed = false
        };

        var result = await userManager.CreateAsync(adminUser, password);

        if (!result.Succeeded)
        {
            logger.LogError("Failed to create admin user: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        // Add user to Admin role (role already exists from .HasData())
        await userManager.AddToRoleAsync(adminUser, "Admin");

        // Log credentials with high visibility
        logger.LogWarning("Admin user created - Username: {Username}", adminUser.UserName);
    }
}
