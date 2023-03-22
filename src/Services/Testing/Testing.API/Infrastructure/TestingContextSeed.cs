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
            if (!context.Tasks.Any())
            {
                context.Tasks.AddRange(
                    new DomainTask[]
                        {
                            new(
                                Guid.NewGuid(),
                                Title.Create("Create Phone Number").Value!,
                                Description.Create(
                                    "Write a function that accepts an array of 10 integers (between 0 and 9), that returns a string of those numbers in the form of a phone number.\r\n\r\nThe returned format must be correct in order to complete this challenge.\r\n\r\nDon't forget the space after the closing parentheses!",
                                    "CreatePhoneNumber(new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0}) // => returns \"(123) 456-7890\"").Value!,
                                Difficulty.Create(1, "Easy").Value!,
                                DomainType.Create(1, "For beginners").Value!,
                                ProgrammingLanguage.Create(1, "CSharp").Value!,
                                SolutionTemplate.Create("public static string CreatePhoneNumber(int[] numbers)\r\n  {\r\n    \r\n  }").Value!,
                                ExecutionCondition.Create("namespace Test\r\n{\r\n    internal class Program\r\n    {\r\n        {code}\r\n\r\n        static void Main(string[] args)\r\n        {\r\n            if (CreatePhoneNumber(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }) != \"(123) 456-7890\")\r\n            {\r\n                throw new Exception(\"Incorrect result\");\r\n            }\r\n\r\n            if (CreatePhoneNumber(new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }) != \"(111) 111-1111\")\r\n            {\r\n                throw new Exception(\"Incorrect result\");\r\n            }\r\n        }\r\n    }\r\n}", TimeSpan.FromSeconds(3)).Value!
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
