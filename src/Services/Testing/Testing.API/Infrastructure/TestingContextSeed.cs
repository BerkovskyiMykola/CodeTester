using Testing.Core.Domain.AggregatesModel.SolutionAggregate;
using Testing.Core.Domain.AggregatesModel.TaskAggregate;
using Testing.Infrastructure.Persistence;
using DomainTask = Testing.Core.Domain.AggregatesModel.TaskAggregate.Task;
using DomainType = Testing.Core.Domain.AggregatesModel.TaskAggregate.Type;
using Task = System.Threading.Tasks.Task;

namespace Testing.API.Infrastructure;

public class TestingContextSeed
{
    public async Task SeedAsync(
    TestingContext context,
    ILogger<TestingContext> logger,
    int retry = 0)
    {
        int retryForAvaiability = retry;

        try
        {
            Guid[] seedGuids = new[] { Guid.NewGuid(), Guid.NewGuid() };
            if (!context.Tasks.Any())
            {
                context.Tasks.AddRange(
                    new DomainTask[] 
                        { 
                            new(
                                seedGuids.ElementAt(0),
                                Title.Create("Easy problem").Value!,
                                Description.Create("Easy problem description", "12 -> 1 2 \n76 -> 7 6").Value!,
                                Difficulty.Create(1, "Easy").Value!,
                                DomainType.Create(1, "For begginers").Value!,
                                ProgrammingLanguage.Create(1, "C#").Value!,
                                SolutionExample.Create("Some description for task #1", "Console.WriteLine($\"{a / 10} {a % 10}\")").Value!,
                                ExecutionCondition.Create("12:1 2; 76: 7 6", TimeSpan.FromSeconds(1)).Value!
                            ),
                            new(
                                seedGuids.ElementAt(1),
                                Title.Create("Mouse").Value!,
                                Description.Create("Mouse is running through the squares", "LLRRRLRL; LLLLLRRRRLLRRR").Value!,
                                Difficulty.Create(2, "Medium").Value!,
                                DomainType.Create(2, "Dynamic programming").Value!,
                                ProgrammingLanguage.Create(2, "C++").Value!,
                                SolutionExample.Create("Some description for task #2", "<solution code #2>").Value!,
                                ExecutionCondition.Create("10, 564: LLLLLRRRRLLRRR; 64, 122: LLRRRLRL", TimeSpan.FromSeconds(2)).Value!
                            )
                        }
                    );

                await context.SaveChangesAsync();
            }


            if (!context.Solutions.Any())
            {
                context.Solutions.AddRange(
                    new Solution[]
                        {
                            new(
                                Guid.NewGuid(),
                                seedGuids.ElementAt(0),
                                User.Create(Guid.NewGuid(), "test@mail.com", "Tester", "Gregor").Value!,
                                SolutionValue.Create("<solution code>").Value!,
                                true
                            ),
                            new(
                                Guid.NewGuid(),
                                seedGuids.ElementAt(0),
                                User.Create(Guid.NewGuid(), "test2@mail.com", "Gate", "Mike").Value!,
                                SolutionValue.Create("<solution code>").Value!,
                                false
                            )
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

                logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(TestingContext));

                await SeedAsync(context, logger, retryForAvaiability);
            }
        }
    }
}
