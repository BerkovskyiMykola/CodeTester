using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Infrastructure;

public class ApplicationContextSeed
{
    public async Task SeedAsync(
        ApplicationContext context,
        ILogger<ApplicationContextSeed> logger,
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
                    Fullname = "Test Test"
                };

                await userManager.CreateAsync(user, "Password@1");
                await userManager.AddToRoleAsync(user, "Admin");

                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            if (retryForAvaiability < 10)
            {
                retryForAvaiability++;

                logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(ApplicationContext));

                await SeedAsync(context, logger, userManager, roleManager, retryForAvaiability);
            }
        }
    }
}
