using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace DataAccess.Data;

public class ApplicationDbContextSeed
{
    public async Task SeedAsync(
        ApplicationDbContext context,
        ILogger<ApplicationDbContextSeed> logger,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        int retry = 0)
    {
        int retryForAvaiability = retry;

        try
        {
            if (!context.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!context.Users.Any())
            {
                var user = new ApplicationUser
                {
                    UserName = "tester@email.com",
                    Email = "tester@email.com",
                    EmailConfirmed = true,
                };

                await userManager.CreateAsync(user, "Password@1");

                await userManager.AddClaimsAsync(user, new Claim[] {
                    new Claim("name", "Tester Admin"),
                    new Claim("given_name", "Tester"),
                    new Claim("family_name", "Admin"),
                    new Claim("website", "http://test.com"),
                });

                await userManager.AddToRoleAsync(user, "Admin");

                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            if (retryForAvaiability < 10)
            {
                retryForAvaiability++;

                logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(ApplicationDbContext));

                await SeedAsync(context, logger, userManager, roleManager, retryForAvaiability);
            }
        }
    }
}
