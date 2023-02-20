using Dapper;
using Npgsql;
using Testing.API.DTOs.Tasks;

namespace Testing.API.Application.Queries.Tasks;

public interface ITaskQueries
{
    Task<TaskResponse> GetTaskAsync(Guid id);

    Task<IEnumerable<TaskResponse>> GetAllTasksAsync();
}

public class TaskQueries : ITaskQueries
{
    private string _connectionString = string.Empty;

    public TaskQueries(IConfiguration configuration)
    {
        _connectionString = configuration["connectionString"]!;
    }

    public async Task<TaskResponse> GetTaskAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var result = await connection.QueryAsync<TaskResponse>(
            @"Select ""Id"" as Id, ""Title_Value"" as Title, ""Description_Text"" as DescriptionText,
                    ""Description_Examples"" as DescriptionExamples, ""Description_SomeCases"" as DescriptionCases,
                    ""Description_Note"" as DescriptionNote, ""Difficulty_Id"" as DifficultyId, ""Difficulty_Name"" as DifficultyName,
                    ""Type_Id"" as TaskTypeId, ""Type_Name"" as TaskTypeName, ""ProgrammingLanguage_Id"" as ProgrammingLanguageId,
                    ""ProgrammingLanguage_Name"" as ProgrammingLanguageName, ""SolutionExample_Description"" as SolutionExampleDescription,
                    ""SolutionExample_Solution"" as SolutionExample, ""ExecutionCondition_Tests"" as ExecutionConditionTests,
                    ""ExecutionCondition_TimeLimit"" as ExecutionTimeLimit, ""CreateDate"" as CreateDate
                    from ""Tasks""
					WHERE ""Id"" = @id"
                , new { id }
            );

        if (result.AsList().Count == 0)
            throw new KeyNotFoundException();

        return result.ElementAt(0);
    }

    public async Task<IEnumerable<TaskResponse>> GetAllTasksAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        return await connection.QueryAsync<TaskResponse>(
            @"Select ""Id"" as Id, ""Title_Value"" as Title, ""Description_Text"" as DescriptionText,
                    ""Description_Examples"" as DescriptionExamples, ""Description_SomeCases"" as DescriptionCases,
                    ""Description_Note"" as DescriptionNote, ""Difficulty_Id"" as DifficultyId, ""Difficulty_Name"" as DifficultyName,
                    ""Type_Id"" as TaskTypeId, ""Type_Name"" as TaskTypeName, ""ProgrammingLanguage_Id"" as ProgrammingLanguageId,
                    ""ProgrammingLanguage_Name"" as ProgrammingLanguageName, ""SolutionExample_Description"" as SolutionExampleDescription,
                    ""SolutionExample_Solution"" as SolutionExample, ""ExecutionCondition_Tests"" as ExecutionConditionTests,
                    ""ExecutionCondition_TimeLimit"" as ExecutionTimeLimit, ""CreateDate"" as CreateDate
                    from ""Tasks"""
        );
    }
}
