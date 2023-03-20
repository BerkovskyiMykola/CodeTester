using Dapper;
using MassTransit.Initializers;
using Testing.API.Application.Queries.Tasks.Models;
using Testing.API.Infrastructure.Models;
using Testing.API.Infrastructure.Services;
using Testing.Core.Domain.AggregatesModel.TaskAggregate;
using Testing.Core.Domain.AggregatesModel.UserAggregate;

namespace Testing.API.Application.Queries.Tasks;

public interface ITaskQueries
{
    Task<PaginationResult<ComplitedCardTaskQueryModel>> GetComplitedCardTasksWithPaginingAsync(
        Guid userId,
        PaginationParameters pagination,
        string? search = null,
        int? difficultyId = null,
        int? programmingLanguageId = null,
        int? typeId = null);

    Task<DetailedTaskQueryModel> GetDetailedTaskAsync(Guid taskId, Guid userId);
}

public class TaskQueries : ITaskQueries
{
    private readonly IDapperService _dapperService;

    public TaskQueries(IDapperService dapperService)
    {
        _dapperService = dapperService;
    }

    public async Task<PaginationResult<ComplitedCardTaskQueryModel>> GetComplitedCardTasksWithPaginingAsync(
        Guid userId,
        PaginationParameters pagination,
        string? search = null,
        int? difficultyId = null,
        int? programmingLanguageId = null,
        int? typeId = null)
    {
        var filter = new List<string>();

        if (!string.IsNullOrWhiteSpace(search)) filter.Add(@$"LOWER(TRIM(""Title_Value"")) LIKE '%' || '{search.Trim().ToLower()}' || '%'");
        if (difficultyId.HasValue) filter.Add(@$"""Difficulty_Id"" = {difficultyId}");
        if (programmingLanguageId.HasValue) filter.Add(@$"""ProgrammingLanguage_Id"" = {programmingLanguageId}");
        if (typeId.HasValue) filter.Add(@$"""Type_Id"" = {typeId}");

        var filterString = filter.Any() ? $"WHERE {string.Join(" AND ", filter)}" : "";

        using var connection = _dapperService.CreateConnection();

        var query =
            @$"SELECT COUNT(*) FROM ""Tasks""
            {filterString};

            SELECT ""Id"", ""Title_Value"", 
            ""Difficulty_Id"", ""Difficulty_Name"",  
            ""ProgrammingLanguage_Id"", ""ProgrammingLanguage_Name"",  
            ""Type_Id"", ""Type_Name"",
            (SELECT COUNT(*) FROM ""Solutions"" WHERE ""Tasks"".""Id"" = ""Solutions"".""TaskId"") as ""CompletedAmount"",
            CASE
                WHEN EXISTS (
		            SELECT 1 FROM ""Solutions"" 
		            WHERE ""Tasks"".""Id"" = ""Solutions"".""TaskId"" AND ""Solutions"".""UserId"" = '{userId}'
	            ) THEN CAST(1 AS boolean)
                ELSE CAST(0 AS boolean)
            END AS ""IsCompleted""
            FROM ""Tasks""
            {filterString}
            ORDER BY ""CreateDate"" DESC
            OFFSET {(pagination.PageNumber - 1) * pagination.PageSize} ROWS FETCH NEXT {pagination.PageSize} ROWS ONLY;";

        var multi = await connection.QueryMultipleAsync(query);
        var totalRowCount = multi.Read<long>().Single();
        var gridDataRows = multi.Read<dynamic>().Select(MapToComplitedCardTaskQueryModel).ToList();

        return new PaginationResult<ComplitedCardTaskQueryModel>(gridDataRows, totalRowCount, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<DetailedTaskQueryModel> GetDetailedTaskAsync(Guid taskId, Guid userId)
    {
        using var connection = _dapperService.CreateConnection();

        var query =
            @$"SELECT ""Id"", ""Title_Value"", 
            ""Description_Text"", ""Description_Examples"", ""Description_SomeCases"", ""Description_Note"",
            ""Difficulty_Id"", ""Difficulty_Name"",  
            ""ProgrammingLanguage_Id"", ""ProgrammingLanguage_Name"",  
            ""Type_Id"", ""Type_Name"",
            ""SolutionTemplate_Value"",
            (SELECT COUNT(*) FROM ""Solutions"" WHERE ""Tasks"".""Id"" = ""Solutions"".""TaskId"") as ""CompletedAmount"",
            CASE
                WHEN EXISTS (
		            SELECT 1 FROM ""Solutions"" 
		            WHERE ""Tasks"".""Id"" = ""Solutions"".""TaskId"" AND ""Solutions"".""UserId"" = '{userId}'
	            ) THEN CAST(1 AS boolean)
                ELSE CAST(0 AS boolean)
            END AS ""IsCompleted""
            FROM ""Tasks""
            WHERE ""Id"" = '{taskId}';";

        var result = await connection.QueryAsync<dynamic>(query);

        if (result.AsList().Count == 0)
            throw new KeyNotFoundException();

        return MapToDetailedTaskQueryModel(result);
    }

    private ComplitedCardTaskQueryModel MapToComplitedCardTaskQueryModel(dynamic obj)
    {
        return new ComplitedCardTaskQueryModel()
        {
            Id = obj.Id,
            Title = obj.Title_Value,
            Difficulty = new DifficultyQueryModel()
            {
                Id = obj.Difficulty_Id,
                Name = obj.Difficulty_Name
            },
            TaskType = new TaskTypeQueryModel()
            {
                Id = obj.Type_Id,
                Name = obj.Type_Name
            },
            ProgrammingLanguage = new ProgrammingLanguageQueryModel()
            {
                Id = obj.ProgrammingLanguage_Id,
                Name = obj.ProgrammingLanguage_Name,
            },
            CompletedAmount = obj.CompletedAmount,
            IsCompleted = obj.IsCompleted,
        };
    }

    private DetailedTaskQueryModel MapToDetailedTaskQueryModel(dynamic obj)
    {
        return new DetailedTaskQueryModel()
        {
            Id = obj[0].Id,
            Title = obj[0].Title_Value,
            Difficulty = new DifficultyQueryModel()
            {
                Id = obj[0].Difficulty_Id,
                Name = obj[0].Difficulty_Name
            },
            TaskType = new TaskTypeQueryModel()
            {
                Id = obj[0].Type_Id,
                Name = obj[0].Type_Name
            },
            ProgrammingLanguage = new ProgrammingLanguageQueryModel()
            {
                Id = obj[0].ProgrammingLanguage_Id,
                Name = obj[0].ProgrammingLanguage_Name,
            },
            Description = new DescriptionQueryModel()
            {
                Text = obj[0].Description_Text,
                Examples = obj[0].Description_Examples,
                SomeCases = obj[0].Description_SomeCases,
                Note = obj[0].Description_Note
            },
            SolutionTemplate = new SolutionTemplateQueryModel
            {
                Value = obj[0].SolutionTemplate_Value,
            },
            CompletedAmount = obj[0].CompletedAmount,
            IsCompleted = obj[0].IsCompleted,
        };
    }
}
