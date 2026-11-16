
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace thebooking.Data;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(
        IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { "Admin", "Manager", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Could not create role '{role}': " +
                        string.Join(", ",
                            result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
