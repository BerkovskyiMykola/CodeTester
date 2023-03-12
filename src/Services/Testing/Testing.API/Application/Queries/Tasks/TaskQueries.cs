using Dapper;
using MassTransit.Initializers;
using Npgsql;
using Testing.API.Application.Queries.Tasks.Models;
using Testing.API.Infrastructure.Models;
using Testing.API.Infrastructure.Services;

namespace Testing.API.Application.Queries.Tasks;

public interface ITaskQueries
{
    Task<PaginationResult<ComplitedCardTaskQueryModel>> GetComplitedCardTasksWithPaginingAsync(
        string userId,
        int pageNumber,
        int pageSize,
        string? search = null,
        int? difficultyId = null,
        int? programmingLanguageId = null,
        int? typeId = null);
}

public class TaskQueries : ITaskQueries
{
    private readonly IDapperService _dapperService;

    public TaskQueries(IDapperService dapperService)
    {
        _dapperService = dapperService;
    }

    public async Task<PaginationResult<ComplitedCardTaskQueryModel>> GetComplitedCardTasksWithPaginingAsync(
        string userId,
        int pageNumber,
        int pageSize,
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
            @$"SELECT COUNT(*) FROM ""Tasks"";

            SELECT ""Id"", ""Title_Value"", 
            ""Difficulty_Id"", ""Difficulty_Name"",  
            ""ProgrammingLanguage_Id"", ""ProgrammingLanguage_Name"",  
            ""Type_Id"", ""Type_Name"",
            (SELECT COUNT(*) FROM ""Solutions"" WHERE ""Tasks"".""Id"" = ""Solutions"".""TaskId"") as ""ComplitedAmount"",
            CASE
                WHEN EXISTS (
		            SELECT 1 FROM ""Solutions"" 
		            WHERE ""Tasks"".""Id"" = ""Solutions"".""TaskId"" AND ""Solutions"".""UserId"" = '{userId}'
	            ) THEN CAST(1 AS boolean)
                ELSE CAST(0 AS boolean)
            END AS ""IsComplited""
            FROM ""Tasks""
            {filterString}
            ORDER BY ""CreateDate"" DESC
            OFFSET {(pageNumber - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY;";

        var multi = await connection.QueryMultipleAsync(query);
        var totalRowCount = multi.Read<long>().Single();
        var gridDataRows = multi.Read<dynamic>().Select(MapToComplitedCardTaskQueryModel).ToList();

        return new PaginationResult<ComplitedCardTaskQueryModel>(gridDataRows, totalRowCount, pageNumber, pageSize);
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
            ComplitedAmount = obj.ComplitedAmount,
            IsComplited = obj.IsComplited,
        };
    }
}
