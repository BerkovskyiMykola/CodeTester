using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

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
