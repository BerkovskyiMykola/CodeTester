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
        int retry = 0)
    {
        int retryForAvaiability = retry;

        try
        {
            if (!context.Users.Any())
            {
                var alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                };

                await userManager.CreateAsync(alice, "Pass123$");

                await userManager.AddClaimsAsync(alice, new Claim[] {
                    new Claim("name", "Alice Smith"),
                    new Claim("given_name", "Alice"),
                    new Claim("family_name", "Smith"),
                    new Claim("website", "http://alice.com"),
                });

                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            if (retryForAvaiability < 10)
            {
                retryForAvaiability++;

                logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(ApplicationDbContext));

                await SeedAsync(context, logger, userManager, retryForAvaiability);
            }
        }
    }
}
