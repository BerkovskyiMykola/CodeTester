using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DataAccess.Data;

public class ApplicationDbContextSeed
{
    public async Task SeedAsync(
        ApplicationDbContext context,
        ILogger<ApplicationDbContextSeed> logger,
        IPasswordHasher<ApplicationUser> passwordHasher,
        int retry = 0)
    {
        int retryForAvaiability = retry;

        try
        {
            if (!context.Users.Any())
            {
                //Add Users

                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            if (retryForAvaiability < 10)
            {
                retryForAvaiability++;

                logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(ApplicationDbContext));

                await SeedAsync(context, logger, passwordHasher, retryForAvaiability);
            }
        }
    }
}
