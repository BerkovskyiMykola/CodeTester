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
                            ),
                            new(
                                Guid.NewGuid(),
                                Title.Create("RGB To Hex Conversion").Value!,
                                Description.Create(
                                    "The rgb function is incomplete. Complete it so that passing in RGB decimal values will result in a hexadecimal representation being returned. Valid decimal values for RGB are 0 - 255. Any values that fall out of that range must be rounded to the closest valid value.",
                                    "rgb(255, 255, 255) # returns FFFFFF\r\nrgb(255, 255, 300) # returns FFFFFF\r\nrgb(0,0,0) # returns 000000\r\nrgb(148, 0, 211) # returns 9400D3",
                                    note: "Your answer should always be 6 characters long, the shorthand with 3 will not work here.").Value!,
                                Difficulty.Create(2, "Medium").Value!,
                                DomainType.Create(2, "Not for beginners").Value!,
                                ProgrammingLanguage.Create(2, "Python").Value!,
                                SolutionTemplate.Create("def rgb(r, g, b):\r\n    # your code here :)\r\n    pass").Value!,
                                ExecutionCondition.Create("{code}\r\n    \r\nif rgb(0, 0, 0) != \"000000\":\r\n    raise Exception(\"testing zero values\")\r\n\r\nif rgb(1, 2, 3) != \"010203\":\r\n    raise Exception(\"testing near zero values\")\r\n\r\nif rgb(255, 255, 255) != \"FFFFFF\":\r\n    raise Exception(\"testing max values\")\r\n    \r\nif rgb(254, 253, 252) != \"FEFDFC\":\r\n    raise Exception(\"testing near max values\")\r\n    \r\nif rgb(-20, 275, 125) != \"00FF7D\":\r\n    raise Exception(\"testing out of range values\")", TimeSpan.FromSeconds(3)).Value!
                            ),
                            new(
                                Guid.NewGuid(),
                                Title.Create("Simple Pig Latin").Value!,
                                Description.Create(
                                    "Move the first letter of each word to the end of it, then add \"ay\" to the end of the word. Leave punctuation marks untouched.",
                                    "pigIt('Pig latin is cool'); // igPay atinlay siay oolcay\r\npigIt('Hello world !');     // elloHay orldway !").Value!,
                                Difficulty.Create(2, "Medium").Value!,
                                DomainType.Create(2, "Not for beginners").Value!,
                                ProgrammingLanguage.Create(3, "JavaScript").Value!,
                                SolutionTemplate.Create("function pigIt(str){\r\n  //Code here\r\n}").Value!,
                                ExecutionCondition.Create("{code}\r\n\r\nif(pigIt('Pig latin is cool') != 'igPay atinlay siay oolcay'){\r\n    throw new Error('Wrong');\r\n}\r\n\r\nif(pigIt('This is my string') != 'hisTay siay ymay tringsay'){\r\n    throw new Error('Wrong');\r\n}", TimeSpan.FromSeconds(3)).Value!
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
