using Dictionary.API.Infrastructure.Entities;

namespace Dictionary.API.Infrastructure;

public class DictionaryContextSeed
{
    public async Task SeedAsync(
        DictionaryContext context,
        ILogger<DictionaryContext> logger,
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

            if (!context.ProgrammingLanguages.Any())
            {
                context.ProgrammingLanguages.AddRange(
                    new ProgrammingLanguage
                    {
                        Name = "C#"
                    },
                    new ProgrammingLanguage
                    {
                        Name = "Python"
                    },
                    new ProgrammingLanguage
                    {
                        Name = "JavaScript"
                    }
                );

                await context.SaveChangesAsync();
            }

            if (!context.TaskTypes.Any())
            {
                context.TaskTypes.AddRange(
                    new TaskType
                    {
                        Name = "Array"
                    },
                    new TaskType
                    {
                        Name = "String"
                    },
                    new TaskType
                    {
                        Name = "Math"
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

                logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(DictionaryContext));

                await SeedAsync(context, logger, retryForAvaiability);
            }
        }
    }
}
