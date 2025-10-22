using System.Security.Cryptography;
using System.Text;
using JasperDocs.WebApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        // Only seed if there are zero users in the database
        if (await userManager.Users.AnyAsync())
        {
            return;
        }

        // Generate a cryptographically secure random password
        var password = GenerateRandomPassword(16);

        // Create admin user
        var adminUser = new ApplicationUser
        {
            Email = "admin@jasperdocs.local",
            UserName = "admin@jasperdocs.local",
            EmailConfirmed = true
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
        logger.LogWarning("Admin user created - Email: {Email} | Password: {Password}", adminUser.Email, password);
    }

    private static string GenerateRandomPassword(int length)
    {
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string symbols = "!@#$%^&*";
        const string allChars = uppercase + lowercase + digits + symbols;

        var password = new StringBuilder(length);

        // Ensure at least one character from each category
        password.Append(uppercase[RandomNumberGenerator.GetInt32(uppercase.Length)]);
        password.Append(lowercase[RandomNumberGenerator.GetInt32(lowercase.Length)]);
        password.Append(digits[RandomNumberGenerator.GetInt32(digits.Length)]);
        password.Append(symbols[RandomNumberGenerator.GetInt32(symbols.Length)]);

        // Fill the rest with random characters from all categories
        for (int i = password.Length; i < length; i++)
        {
            password.Append(allChars[RandomNumberGenerator.GetInt32(allChars.Length)]);
        }

        // Shuffle the password to avoid predictable patterns
        return Shuffle(password.ToString());
    }

    private static string Shuffle(string input)
    {
        var chars = input.ToCharArray();
        for (int i = chars.Length - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
        return new string(chars);
    }
}
