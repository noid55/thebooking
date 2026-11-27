
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace thebooking.Data;

public static class AdminSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        var configuration = serviceProvider
            .GetRequiredService<IConfiguration>();

        string email = configuration["AdminAccount:Email"]
            ?? throw new InvalidOperationException("Admin email is missing.");

        string password = configuration["AdminAccount:Password"]
            ?? throw new InvalidOperationException("Admin password is missing.");

        var admin = await userManager.FindByEmailAsync(email);

        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    "Could not create Admin: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            var result = await userManager.AddToRoleAsync(admin, "Admin");

            if (!result.Succeeded)
            {
                throw new Exception(
                    "Could not assign Admin role: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
