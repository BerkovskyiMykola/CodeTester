using Dapper;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql;
using Testing.API.Application.Queries.Tasks.Models;
using Testing.API.DTOs.Tasks;

namespace Testing.API.Application.Queries.Tasks;

public interface ITaskQueries
{
    Task<TaskQueriesModel> GetTaskAsync(Guid id);

    Task<IEnumerable<TaskQueriesModel>> GetAllTasksAsync();
}

public class TaskQueries : ITaskQueries
{
    private string _connectionString = string.Empty;

    public TaskQueries(IConfiguration configuration)
    {
        _connectionString = configuration["connectionString"]!;
    }

    public async Task<TaskQueriesModel> GetTaskAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var result = await connection.QueryAsync<dynamic>(
            @"Select ""Id"" as ""Id"", ""Title_Value"" as ""Title"", ""Description_Text"" as ""DescriptionText"",
                    ""Description_Examples"" as ""DescriptionExamples"", ""Description_SomeCases"" as ""DescriptionCases"",
                    ""Description_Note"" as ""DescriptionNote"", ""Difficulty_Id"" as ""DifficultyId"", ""Difficulty_Name"" as ""DifficultyName"",
                    ""Type_Id"" as ""TaskTypeId"", ""Type_Name"" as ""TaskTypeName"", ""ProgrammingLanguage_Id"" as ""ProgrammingLanguageId"",
                    ""ProgrammingLanguage_Name"" as ""ProgrammingLanguageName"", ""SolutionExample_Description"" as ""SolutionExampleDescription"",
                    ""SolutionExample_Solution"" as ""SolutionExample"", ""ExecutionCondition_Tests"" as ""ExecutionConditionTests"",
                    ""ExecutionCondition_TimeLimit"" as ""ExecutionTimeLimit"", ""CreateDate"" as ""CreateDate""
                    from ""Tasks""
					WHERE ""Id"" = @id"
                , new { id }
            );

        if (result.AsList().Count == 0)
            throw new KeyNotFoundException();

        return MapToTaskQueriesModel(result.ElementAt(0));
    }

    public async Task<IEnumerable<TaskQueriesModel>> GetAllTasksAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        var result = await connection.QueryAsync<dynamic>(
            @"Select ""Id"" as ""Id"", ""Title_Value"" as ""Title"", ""Description_Text"" as ""DescriptionText"",
                    ""Description_Examples"" as ""DescriptionExamples"", ""Description_SomeCases"" as ""DescriptionCases"",
                    ""Description_Note"" as ""DescriptionNote"", ""Difficulty_Id"" as ""DifficultyId"", ""Difficulty_Name"" as ""DifficultyName"",
                    ""Type_Id"" as ""TaskTypeId"", ""Type_Name"" as ""TaskTypeName"", ""ProgrammingLanguage_Id"" as ""ProgrammingLanguageId"",
                    ""ProgrammingLanguage_Name"" as ""ProgrammingLanguageName"", ""SolutionExample_Description"" as ""SolutionExampleDescription"",
                    ""SolutionExample_Solution"" as ""SolutionExample"", ""ExecutionCondition_Tests"" as ""ExecutionConditionTests"",
                    ""ExecutionCondition_TimeLimit"" as ""ExecutionTimeLimit"", ""CreateDate"" as ""CreateDate""
                    from ""Tasks"""
        );

        return result.Select(x => (TaskQueriesModel)MapToTaskQueriesModel(x)).AsEnumerable();
    }

    private TaskQueriesModel MapToTaskQueriesModel(dynamic obj)
    {
        return new TaskQueriesModel()
        {
            Id = obj.Id,
            Title = obj.Title,
            Description = new TaskQueriesModelDescription() 
            {
                Examples = obj.DescriptionExamples,
                SomeCases = obj.DescriptionCases,
                Note = obj.DescriptionNote,
                Text = obj.DescriptionText
            },
            Difficulty = new TaskQueriesModelDifficulty()
            {
                Id = obj.DifficultyId,
                Name = obj.DifficultyName
            },
            TaskType = new TaskQueriesModelTaskType()
            {
                Id = obj.TaskTypeId,
                Name = obj.TaskTypeName
            },
            ProgrammingLanguage = new TaskQueriesModelProgrammingLanguage()
            {
                Id = obj.ProgrammingLanguageId,
                Name = obj.ProgrammingLanguageName,
            },
            SolutionExample = new TaskQueriesModelSolutionExample()
            {
                Description = obj.SolutionExampleDescription,
                Solution = obj.SolutionExample
            },
            ExecutionCondition = new TaskQueriesModelExecutionCondition()
            {
                Tests = obj.ExecutionConditionTests,
                TimeLimit = obj.ExecutionTimeLimit,
            },
            CreateDate = obj.CreateDate
        };
    }
}
