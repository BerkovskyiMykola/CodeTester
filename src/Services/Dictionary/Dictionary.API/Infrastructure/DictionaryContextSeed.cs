﻿using Dictionary.API.Infrastructure.Entities;

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
                    new Difficulty("Easy"),
                    new Difficulty("Medium"),
                    new Difficulty("Hard")
                );

                await context.SaveChangesAsync();
            }

            if (!context.ProgrammingLanguages.Any())
            {
                context.ProgrammingLanguages.AddRange(
                    new ProgrammingLanguage("CSharp"),
                    new ProgrammingLanguage("Python"),
                    new ProgrammingLanguage("JavaScript"),
                    new ProgrammingLanguage("FSharp"),
                    new ProgrammingLanguage("Java"),
                    new ProgrammingLanguage("Golang")
                );

                await context.SaveChangesAsync();
            }

            if (!context.TaskTypes.Any())
            {
                context.TaskTypes.AddRange(
                    new TaskType("String"),
                    new TaskType("Math"),
                    new TaskType("Sorting"),
                    new TaskType("Array")
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
