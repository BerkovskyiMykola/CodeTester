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
                                DomainType.Create(1, "String").Value!,
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
                                DomainType.Create(2, "Math").Value!,
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
                                DomainType.Create(2, "String").Value!,
                                ProgrammingLanguage.Create(3, "JavaScript").Value!,
                                SolutionTemplate.Create("function pigIt(str){\r\n  //Code here\r\n}").Value!,
                                ExecutionCondition.Create("{code}\r\n\r\nif(pigIt('Pig latin is cool') != 'igPay atinlay siay oolcay'){\r\n    throw new Error('Wrong');\r\n}\r\n\r\nif(pigIt('This is my string') != 'hisTay siay ymay tringsay'){\r\n    throw new Error('Wrong');\r\n}", TimeSpan.FromSeconds(3)).Value!
                            ),
                            new(
                                Guid.NewGuid(),
                                Title.Create("Sort the odd").Value!,
                                Description.Create(
                                    "You will be given an array of numbers. You have to sort the odd numbers in ascending order while leaving the even numbers at their original positions.",
                                    "[7, 1]  =>  [1, 7]\r\n[5, 8, 6, 3, 4]  =>  [3, 8, 6, 5, 4]\r\n[9, 8, 7, 6, 5, 4, 3, 2, 1, 0]  =>  [1, 8, 3, 6, 5, 4, 7, 2, 9, 0]").Value!,
                                Difficulty.Create(2, "Medium").Value!,
                                DomainType.Create(1, "Sorting").Value!,
                                ProgrammingLanguage.Create(1, "CSharp").Value!,
                                SolutionTemplate.Create("public static int[] SortArray(int[] array)\r\n{\r\n  return array;\r\n}").Value!,
                                ExecutionCondition.Create("namespace Test\r\n{\r\n    internal class Program\r\n    {\r\n        {code}\r\n\r\n        static void Main(string[] args)\r\n        {\r\n            if (!Enumerable.SequenceEqual(SortArray(new int[] { 5, 3, 2, 8, 1, 4 }), new int[] { 1, 3, 2, 8, 5, 4 }))\r\n            {\r\n                throw new Exception(\"Wrong\");\r\n            }\r\n\r\n            if (!Enumerable.SequenceEqual(SortArray(new int[] { 5, 3, 1, 8, 0 }), new int[] { 1, 3, 5, 8, 0 }))\r\n            {\r\n                throw new Exception(\"Wrong\");\r\n            }\r\n\r\n            if (!Enumerable.SequenceEqual(SortArray(new int[] { }), new int[] { }))\r\n            {\r\n                throw new Exception(\"Wrong\");\r\n            }\r\n        }\r\n    }\r\n}", TimeSpan.FromSeconds(3)).Value!
                            ),
                            new(
                                Guid.NewGuid(),
                                Title.Create("PaginationHelper").Value!,
                                Description.Create(
                                    "For this exercise you will be strengthening your page-fu mastery. You will complete the PaginationHelper class, which is a utility class helpful for querying paging information related to an array.\r\n\r\nThe class is designed to take in an array of values and an integer indicating how many items will be allowed per each page. The types of values contained within the collection/array are not relevant.",
                                    "helper = PaginationHelper(['a','b','c','d','e','f'], 4)\r\nhelper.page_count() # should == 2\r\nhelper.item_count() # should == 6\r\nhelper.page_item_count(0)  # should == 4\r\nhelper.page_item_count(1) # last page - should == 2\r\nhelper.page_item_count(2) # should == -1 since the page is invalid\r\n\r\n# page_index takes an item index and returns the page that it belongs on\r\nhelper.page_index(5) # should == 1 (zero based index)\r\nhelper.page_index(2) # should == 0\r\nhelper.page_index(20) # should == -1\r\nhelper.page_index(-10) # should == -1 because negative indexes are invalid").Value!,
                                Difficulty.Create(3, "Hard").Value!,
                                DomainType.Create(2, "Array").Value!,
                                ProgrammingLanguage.Create(2, "Python").Value!,
                                SolutionTemplate.Create("# TODO: complete this class\r\n\r\nclass PaginationHelper:\r\n\r\n    # The constructor takes in an array of items and a integer indicating\r\n    # how many items fit within a single page\r\n    def __init__(self, collection, items_per_page):\r\n        pass\r\n\r\n    # returns the number of items within the entire collection\r\n    def item_count(self):\r\n        pass\r\n\r\n    # returns the number of pages\r\n    def page_count(self):\r\n        pass\r\n\r\n    # returns the number of items on the current page. page_index is zero based\r\n    # this method should return -1 for page_index values that are out of range\r\n    def page_item_count(self, page_index):\r\n        pass").Value!,
                                ExecutionCondition.Create("{code}\r\n      \r\n# -----------------------\r\n      \r\ncollection = range(1,25)\r\nhelper = PaginationHelper(collection, 10)\r\n\r\nif helper.page_count() != 3:\r\n    raise Exception('page_count is returning incorrect value')\r\n    \r\nif helper.page_index(23) != 2:\r\n    raise Exception('page_index returned incorrect value')\r\n    \r\nif helper.item_count() != 24:\r\n    raise Exception('item_count returned incorrect value')", TimeSpan.FromSeconds(3)).Value!
                            ),
                            new(
                                Guid.NewGuid(),
                                Title.Create("Replace With Alphabet Position").Value!,
                                Description.Create(
                                    "Welcome.\r\n\r\nIn this task you are required to, given a string, replace every letter with its position in the alphabet.\r\n\r\nIf anything in the text isn't a letter, ignore it and don't return it.\r\n\r\n\"a\" = 1, \"b\" = 2, etc.",
                                    "alphabetPosition(\"The sunset sets at twelve o' clock.\")\r\nShould return \"20 8 5 19 21 14 19 5 20 19 5 20 19 1 20 20 23 5 12 22 5 15 3 12 15 3 11\" ( as a string )").Value!,
                                Difficulty.Create(2, "Medium").Value!,
                                DomainType.Create(2, "String").Value!,
                                ProgrammingLanguage.Create(3, "JavaScript").Value!,
                                SolutionTemplate.Create("function alphabetPosition(text) {\r\n  return text;\r\n}").Value!,
                                ExecutionCondition.Create("{code}\r\n\r\nif(alphabetPosition(\"The sunset sets at twelve o' clock.\") != \"20 8 5 19 21 14 19 5 20 19 5 20 19 1 20 20 23 5 12 22 5 15 3 12 15 3 11\"){\r\n    throw new Exception(\"Wrong\")\r\n}\r\n\r\nif(alphabetPosition(\"The narwhal bacons at midnight.\") != \"20 8 5 14 1 18 23 8 1 12 2 1 3 15 14 19 1 20 13 9 4 14 9 7 8 20\"){\r\n    throw new Exception(\"Wrong\")\r\n}", TimeSpan.FromSeconds(3)).Value!
                            ),
                            new(
                                Guid.NewGuid(),
                                Title.Create("Convert string to camel case").Value!,
                                Description.Create(
                                    "Complete the method/function so that it converts dash/underscore delimited words into camel casing. The first word within the output should be capitalized only if the original word was capitalized (known as Upper Camel Case, also often referred to as Pascal case). The next words should be always capitalized.",
                                    "\"the-stealth-warrior\" gets converted to \"theStealthWarrior\"\r\n\r\n\"The_Stealth_Warrior\" gets converted to \"TheStealthWarrior\"\r\n\r\n\"The_Stealth-Warrior\" gets converted to \"TheStealthWarrior\"").Value!,
                                Difficulty.Create(1, "Easy").Value!,
                                DomainType.Create(1, "String").Value!,
                                ProgrammingLanguage.Create(4, "FSharp").Value!,
                                SolutionTemplate.Create("let toCamelCase (text : string) =\r\n  // your code here").Value!,
                                ExecutionCondition.Create("{code}\r\n\r\nif (toCamelCase \"\" <> \"\") then raise (System.Exception(\"Wrong\"))\r\n\r\nif (toCamelCase \"the_stealth_warrior\" <> \"theStealthWarrior\") then raise (System.Exception(\"Wrong\"))\r\n\r\nif (toCamelCase \"The-Stealth-Warrior\" <> \"TheStealthWarrior\") then raise (System.Exception(\"Wrong\"))\r\n\r\nif (toCamelCase \"A-B-C\" <> \"ABC\") then raise (System.Exception(\"Wrong\"))", TimeSpan.FromSeconds(3)).Value!
                            ),
                            new(
                                Guid.NewGuid(),
                                Title.Create("Valid Parentheses").Value!,
                                Description.Create(
                                    "Write a function that takes a string of parentheses, and determines if the order of the parentheses is valid. The function should return true if the string is valid, and false if it's invalid.",
                                    "\"()\"              =>  true\r\n\")(()))\"          =>  false\r\n\"(\"               =>  false\r\n\"(())((()())())\"  =>  true").Value!,
                                Difficulty.Create(1, "Easy").Value!,
                                DomainType.Create(1, "String").Value!,
                                ProgrammingLanguage.Create(5, "Java").Value!,
                                SolutionTemplate.Create("public static boolean validParentheses(String parens)\r\n{\r\n  //Put code below\r\n}").Value!,
                                ExecutionCondition.Create("public class Program {\r\n    \r\n    {code}\r\n\r\n    public static void main(String args[]) throws Exception {\r\n        \r\n        if(validParentheses(\"()\") != true){\r\n           throw new Exception(\"Wrong\"); \r\n        }\r\n        if(validParentheses(\"())\") != false){\r\n           throw new Exception(\"Wrong\"); \r\n        }\r\n        if(validParentheses(\"32423(sgsdg)\") != true){\r\n           throw new Exception(\"Wrong\"); \r\n        }\r\n        if(validParentheses(\"(dsgdsg))2432\") != false){\r\n           throw new Exception(\"Wrong\"); \r\n        }\r\n        if(validParentheses(\"adasdasfa\") != true){\r\n           throw new Exception(\"Wrong\"); \r\n        }\r\n    }\r\n}", TimeSpan.FromSeconds(3)).Value!
                            ),
                            new(
                                Guid.NewGuid(),
                                Title.Create("Valid Braces").Value!,
                                Description.Create(
                                    "Write a function that takes a string of braces, and determines if the order of the braces is valid. It should return true if the string is valid, and false if it's invalid.\r\n\r\nThis Kata is similar to the Valid Parentheses Kata, but introduces new characters: brackets [], and curly braces {}. Thanks to @arnedag for the idea!\r\n\r\nAll input strings will be nonempty, and will only consist of parentheses, brackets and curly braces: ()[]{}.\r\n\r\nWhat is considered Valid?\r\nA string of braces is considered valid if all braces are matched with the correct brace.",
                                    "\"(){}[]\"   =>  True\r\n\"([{}])\"   =>  True\r\n\"(}\"       =>  False\r\n\"[(])\"     =>  False\r\n\"[({})](]\" =>  False").Value!,
                                Difficulty.Create(1, "Easy").Value!,
                                DomainType.Create(1, "String").Value!,
                                ProgrammingLanguage.Create(6, "Golang").Value!,
                                SolutionTemplate.Create("func ValidBraces(str string) bool {\r\n\r\n  return false\r\n}").Value!,
                                ExecutionCondition.Create("package main\r\n\r\nimport (\r\n\t\"errors\"\r\n\t\"fmt\"\r\n)\r\n\r\n{code}\r\n\r\nfunc main() {\r\n    if ValidBraces(\"(){}[]\") != true {\r\n        fmt.Println(errors.New(\"Wrong\"))\r\n    }\r\n    if ValidBraces(\"([{}])\") != true {\r\n        fmt.Println(errors.New(\"Wrong\"))\r\n    }\r\n    if ValidBraces(\"(}\") != false {\r\n        fmt.Println(errors.New(\"Wrong\"))\r\n    }\r\n    if ValidBraces(\"[({)](]\") != false {\r\n        fmt.Println(errors.New(\"Wrong\"))\r\n    }\r\n}", TimeSpan.FromSeconds(3)).Value!
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
