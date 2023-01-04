using Dictionary.API.Entities;
using Microsoft.AspNetCore.Identity;

namespace Dictionary.API.Persistence;

public class DictionaryDBContextSeed
{
    public async Task SeedAsync(
        DictionaryDBContext context,
        ILogger<DictionaryDBContext> logger,
        int retry = 0)
    {
        int retryForAvaiability = retry;

        try
        {
            if (!context.Difficulties.Any())
            {
                context.Difficulties.AddRange(
                    new Difficulty
                    {
                        Name = "Easy"
                    },
                    new Difficulty
                    {
                        Name = "Medium"
                    },
                    new Difficulty
                    {
                        Name = "Hard"
                    }
                );

                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            if (retryForAvaiability < 10)
            {
                retryForAvaiability++;

                logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(DictionaryDBContext));

                await SeedAsync(context, logger, retryForAvaiability);
            }
        }
    }
}
