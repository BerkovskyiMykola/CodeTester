using Common.Models.Pagination;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Testing.API.DTOs.Base;
using Testing.API.DTOs.Query;
using Testing.API.DTOs.Solutions;
using Testing.API.DTOs.Tasks;
using Testing.API.Infrastructure.Services;

namespace Testing.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Roles = "User")]
public class CodeChallengeController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly IDapperService _dapperService;

    public CodeChallengeController(
        IIdentityService identityService,
        IDapperService dapperService)
    {
        _identityService = identityService;
        _dapperService = dapperService;
    }

    #region GET /tasks

    [HttpGet("tasks")]
    public async Task<IActionResult> GetTasks(
        [FromQuery] TasksFiltrationParameters filtration,
        [FromQuery] PaginationParameters pagination)
    {
        var userId = _identityService.GetUserIdentity();

        if (userId == null)
        {
            return NotFound("No user found");
        }

        var filter = new List<string>();

        if (!string.IsNullOrWhiteSpace(filtration.Search)) filter.Add(@$"LOWER(TRIM(""Title_Value"")) LIKE '%' || '{filtration.Search.Trim().ToLower()}' || '%'");
        if (filtration.DifficultyId.HasValue) filter.Add(@$"""Difficulty_Id"" = {filtration.DifficultyId}");
        if (filtration.ProgrammingLanguageId.HasValue) filter.Add(@$"""ProgrammingLanguage_Id"" = {filtration.ProgrammingLanguageId}");
        if (filtration.TypeId.HasValue) filter.Add(@$"""Type_Id"" = {filtration.TypeId}");
        if (filtration.IsCompleted.HasValue) filter.Add(@$"""IsCompleted"" IS {filtration.IsCompleted}");

        var filterString = filter.Any() ? $"WHERE {string.Join(" AND ", filter)}" : "";

        var baseQuery =
            $@"SELECT ""Id"", ""Title_Value"", 
            ""Difficulty_Id"", ""Difficulty_Name"",  
            ""ProgrammingLanguage_Id"", ""ProgrammingLanguage_Name"",  
            ""Type_Id"", ""Type_Name"",
            (SELECT COUNT(DISTINCT ""Solutions"".""UserId"") FROM ""Solutions"" WHERE ""Tasks"".""Id"" = ""Solutions"".""TaskId"" AND ""Solutions"".""Success"" IS TRUE) as ""CompletedAmount"",
            CASE
                WHEN EXISTS (
		            SELECT 1 FROM ""Solutions"" 
		            WHERE ""Tasks"".""Id"" = ""Solutions"".""TaskId"" AND ""Solutions"".""UserId"" = '{userId}' AND ""Solutions"".""Success"" IS TRUE
	            ) THEN CAST(1 AS boolean)
                ELSE CAST(0 AS boolean)
            END AS ""IsCompleted""
            FROM ""Tasks""
            ORDER BY ""CreateDate"" DESC";

        var query =
            @$"SELECT COUNT(*) FROM ({baseQuery}) ""Nested""
            {filterString};

            SELECT * FROM ({baseQuery}) ""Nested""
            {filterString}
            OFFSET {(pagination.PageNumber - 1) * pagination.PageSize} ROWS FETCH NEXT {pagination.PageSize} ROWS ONLY;";

        using var connection = _dapperService.CreateConnection();

        var multi = await connection.QueryMultipleAsync(query);
        var totalRowCount = multi.Read<long>().Single();
        var gridDataRows = multi.Read<dynamic>().Select(MapToTaskSummaryWithStatsResponse).ToList();

        return Ok(new PaginationResult<TaskSummaryWithStatsResponse>(gridDataRows, totalRowCount, pagination.PageNumber, pagination.PageSize));
    }

    private TaskSummaryWithStatsResponse MapToTaskSummaryWithStatsResponse(dynamic obj)
    {
        return new TaskSummaryWithStatsResponse()
        {
            Id = obj.Id,
            Title = obj.Title_Value,
            Difficulty = new DifficultyResponse()
            {
                Id = obj.Difficulty_Id,
                Name = obj.Difficulty_Name
            },
            TaskType = new TaskTypeResponse()
            {
                Id = obj.Type_Id,
                Name = obj.Type_Name
            },
            ProgrammingLanguage = new ProgrammingLanguageResponse()
            {
                Id = obj.ProgrammingLanguage_Id,
                Name = obj.ProgrammingLanguage_Name,
            },
            CompletedAmount = obj.CompletedAmount,
            IsCompleted = obj.IsCompleted,
        };
    }

    #endregion

    #region GET /task/{id}

    [HttpGet("tasks/{id}")]
    public async Task<IActionResult> GetTask(Guid id)
    {
        var userId = _identityService.GetUserIdentity();

        if (userId == null)
        {
            return NotFound("No user found");
        }

        var query =
            $@"SELECT ""Id"", ""Title_Value"", 
            ""Description_Text"", ""Description_Examples"", ""Description_SomeCases"", ""Description_Note"",
            ""Difficulty_Id"", ""Difficulty_Name"",  
            ""ProgrammingLanguage_Id"", ""ProgrammingLanguage_Name"",  
            ""Type_Id"", ""Type_Name"",
            ""SolutionTemplate_Value"",
            (SELECT COUNT(DISTINCT ""Solutions"".""UserId"") FROM ""Solutions"" WHERE ""Tasks"".""Id"" = ""Solutions"".""TaskId"" AND ""Solutions"".""Success"" IS TRUE) as ""CompletedAmount"",
            CASE
                WHEN EXISTS (
		            SELECT 1 FROM ""Solutions"" 
		            WHERE ""Tasks"".""Id"" = ""Solutions"".""TaskId"" AND ""Solutions"".""UserId"" = '{userId}' AND ""Solutions"".""Success"" IS TRUE
	            ) THEN CAST(1 AS boolean)
                ELSE CAST(0 AS boolean)
            END AS ""IsCompleted""
            FROM ""Tasks""
            WHERE ""Id"" = '{id}'
            FETCH FIRST 1 ROWS ONLY;";

        using var connection = _dapperService.CreateConnection();

        var result = await connection.QueryAsync<dynamic>(query);

        if (result.Count() == 0)
        {
            return NotFound("No task found");
        }

        return Ok(MapToTaskDetailsWithStatsResponse(result.First()));
    }

    private TaskDetailsWithStatsResponse MapToTaskDetailsWithStatsResponse(dynamic obj)
    {
        return new TaskDetailsWithStatsResponse()
        {
            Id = obj.Id,
            Title = obj.Title_Value,
            Difficulty = new DifficultyResponse()
            {
                Id = obj.Difficulty_Id,
                Name = obj.Difficulty_Name
            },
            TaskType = new TaskTypeResponse()
            {
                Id = obj.Type_Id,
                Name = obj.Type_Name
            },
            ProgrammingLanguage = new ProgrammingLanguageResponse()
            {
                Id = obj.ProgrammingLanguage_Id,
                Name = obj.ProgrammingLanguage_Name,
            },
            Description = new TaskDescriptionResponse()
            {
                Text = obj.Description_Text,
                Examples = obj.Description_Examples,
                SomeCases = obj.Description_SomeCases,
                Note = obj.Description_Note
            },
            SolutionTemplate = obj.SolutionTemplate_Value,
            CompletedAmount = obj.CompletedAmount,
            IsCompleted = obj.IsCompleted,
        };
    }

    #endregion

    #region GET /task/{id}/last-success-solution-attempt and /task/{id}/solution-attempts

    [HttpGet("tasks/{id}/last-success-solution-attempt")]
    public async Task<IActionResult> GetLastSuccessfulSolutionAttemptOfTask(Guid id)
    {
        var userId = _identityService.GetUserIdentity();

        if (userId == null)
        {
            return NotFound("No user found");
        }

        var query =
            $@"SELECT ""Id"", ""Value_Value"", ""Success"", ""CreateDate""
            FROM ""Solutions""
            WHERE ""UserId"" = '{userId}' AND ""TaskId"" = '{id}' AND ""Success"" IS TRUE
            ORDER BY ""CreateDate"" DESC
            FETCH FIRST 1 ROWS ONLY;";

        using var connection = _dapperService.CreateConnection();

        var result = await connection.QueryAsync<dynamic>(query);

        if (result.Count() == 0)
        {
            return NotFound("No solution attempt found");
        }

        return Ok(MapToSolutionAppemptResponse(result.First()));
    }

    [HttpGet("tasks/{id}/solution-attempts")]
    public async Task<IActionResult> GetSolutionAttemptsOfTask(
        Guid id,
        [FromQuery] PaginationParameters pagination)
    {
        var userId = _identityService.GetUserIdentity();

        if (userId == null)
        {
            return NotFound("No user found");
        }

        var baseQuery =
            $@"SELECT ""Id"", ""Value_Value"", ""Success"", ""CreateDate""
            FROM ""Solutions""
            WHERE ""UserId"" = '{userId}' AND ""TaskId"" = '{id}'
            ORDER BY ""CreateDate"" DESC";

        var query =
            @$"SELECT COUNT(*) FROM ({baseQuery}) ""Nested"";

            SELECT * FROM ({baseQuery}) ""Nested""
            OFFSET {(pagination.PageNumber - 1) * pagination.PageSize} ROWS FETCH NEXT {pagination.PageSize} ROWS ONLY;";

        using var connection = _dapperService.CreateConnection();

        var multi = await connection.QueryMultipleAsync(query);
        var totalRowCount = multi.Read<long>().Single();
        var gridDataRows = multi.Read<dynamic>().Select(MapToSolutionAppemptResponse).ToList();

        return Ok(new PaginationResult<SolutionAppemptResponse>(gridDataRows, totalRowCount, pagination.PageNumber, pagination.PageSize));
    }

    private SolutionAppemptResponse MapToSolutionAppemptResponse(dynamic obj)
    {
        return new SolutionAppemptResponse()
        {
            Id = obj.Id,
            Code = obj.Value_Value,
            Success = obj.Success,
            CreateDate = obj.CreateDate,
        };
    }

    #endregion
}
