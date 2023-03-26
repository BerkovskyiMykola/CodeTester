using Common.Models.Pagination;
using Dapper;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Testing.API.DTOs.Base;
using Testing.API.DTOs.Query;
using Testing.API.DTOs.Solutions;
using Testing.API.DTOs.Tasks;
using Testing.API.DTOs.Users;
using Testing.API.Infrastructure.Services;
using Testing.API.Infrastructure.Services.ExecutionCompiler;
using Testing.API.Infrastructure.Services.ExecutionGenerator;
using Testing.Core.Domain.AggregatesModel.SolutionAggregate;
using Testing.Core.Domain.Repositories;

namespace Testing.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Roles = "User")]
public class CodeChallengeController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly IDapperService _dapperService;
    private readonly ITaskRepository _taskRepository;
    private readonly ISolutionRepository _solutionRepository;
    private readonly IExecutionGenerator _executionGenerator;
    private readonly IExecutionCompiler _executionCompiler;
    private readonly ApiBehaviorOptions _apiBehaviorOptions;

    public CodeChallengeController(
        IIdentityService identityService,
        IDapperService dapperService,
        ITaskRepository taskRepository,
        ISolutionRepository solutionRepository,
        IExecutionGenerator executionGenerator,
        IExecutionCompiler executionCompiler,
        IOptions<ApiBehaviorOptions> apiBehaviorOptions)
    {
        _identityService = identityService;
        _dapperService = dapperService;
        _taskRepository = taskRepository;
        _solutionRepository = solutionRepository;
        _executionGenerator = executionGenerator;
        _executionCompiler = executionCompiler;
        _apiBehaviorOptions = apiBehaviorOptions.Value;
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

        var isTaskExistQuery =
            $@"SELECT CAST(1 AS boolean)
            FROM ""Tasks""
            WHERE ""Tasks"".""Id"" = '{id}'
            FETCH FIRST 1 ROWS ONLY;";

        var query =
            $@"SELECT ""Id"", ""Value_Value"", ""Success"", ""CreateDate""
            FROM ""Solutions""
            WHERE ""UserId"" = '{userId}' AND ""TaskId"" = '{id}' AND ""Success"" IS TRUE
            ORDER BY ""CreateDate"" DESC
            FETCH FIRST 1 ROWS ONLY;";

        using var connection = _dapperService.CreateConnection();

        var isTaskExist = await connection.QueryAsync<dynamic>(isTaskExistQuery);

        if (isTaskExist.Count() == 0)
        {
            return NotFound("No task found");
        }

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

        var isTaskExistQuery =
            $@"SELECT CAST(1 AS boolean)
            FROM ""Tasks""
            WHERE ""Tasks"".""Id"" = '{id}'
            FETCH FIRST 1 ROWS ONLY;";

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

        var isTaskExist = await connection.QueryAsync<dynamic>(isTaskExistQuery);

        if (isTaskExist.Count() == 0)
        {
            return NotFound("No task found");
        }

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

    #region GET /tasks/{id}/solutions

    [HttpGet("tasks/{id}/solutions")]
    public async Task<IActionResult> GetSolutionsOfTask(
        Guid id,
        [FromQuery] PaginationParameters pagination)
    {
        var userId = _identityService.GetUserIdentity();

        if (userId == null)
        {
            return NotFound("No user found");
        }

        var isTaskExistQuery =
            $@"SELECT CAST(1 AS boolean)
            FROM ""Tasks""
            WHERE ""Tasks"".""Id"" = '{id}'
            FETCH FIRST 1 ROWS ONLY;";

        var isTaskSolvedQuery =
            $@"SELECT CAST(1 AS boolean) 
            FROM ""Solutions"" 
		    WHERE ""Solutions"".""TaskId"" = '{id}' AND ""Solutions"".""UserId"" = '{userId}' AND ""Solutions"".""Success"" IS TRUE
            FETCH FIRST 1 ROWS ONLY;";

        var baseQuery =
            $@"SELECT ""Nested2"".""Id"", ""Value_Value"", ""Success"", ""CreateDate"", ""UserId"", ""Profile_Lastname"", ""Profile_Firstname"" FROM ""Users""
            INNER JOIN (
	            SELECT * FROM (
		            SELECT 
		            ""Id"", ""Value_Value"", ""Success"", ""TaskId"", ""UserId"", ""CreateDate"", 
		            ROW_NUMBER() OVER (PARTITION BY ""UserId"" ORDER BY ""CreateDate"" DESC) as ""Row"" 
		            FROM ""Solutions""
		            WHERE ""Success"" IS TRUE
	            ) ""Nested""
	            WHERE ""Row"" = 1
            ) ""Nested2"" ON ""Users"".""Id"" = ""Nested2"".""UserId""
            ORDER BY ""CreateDate"" DESC";

        var query =
            @$"SELECT COUNT(*) FROM ({baseQuery}) ""Nested"";

            SELECT * FROM ({baseQuery}) ""Nested""
            OFFSET {(pagination.PageNumber - 1) * pagination.PageSize} ROWS FETCH NEXT {pagination.PageSize} ROWS ONLY;";

        using var connection = _dapperService.CreateConnection();

        var isTaskExist = await connection.QueryAsync<dynamic>(isTaskExistQuery);

        if (isTaskExist.Count() == 0)
        {
            return NotFound("No task found");
        }

        var isTaskSolved = await connection.QueryAsync<dynamic>(isTaskSolvedQuery);

        if (isTaskSolved.Count() == 0)
        {
            return Forbid();
        }

        var multi = await connection.QueryMultipleAsync(query);
        var totalRowCount = multi.Read<long>().Single();
        var gridDataRows = multi.Read<dynamic>().Select(MapToSolutionAppemptWithUserInfoResponse).ToList();

        return Ok(new PaginationResult<SolutionAppemptWithUserInfoResponse>(gridDataRows, totalRowCount, pagination.PageNumber, pagination.PageSize));
    }

    private SolutionAppemptWithUserInfoResponse MapToSolutionAppemptWithUserInfoResponse(dynamic obj)
    {
        return new SolutionAppemptWithUserInfoResponse()
        {
            Id = obj.Id,
            Code = obj.Value_Value,
            CreateDate = obj.CreateDate,
            Success = obj.Success,
            User = new UserResponse
            {
                Id = obj.UserId,
                Firstname = obj.Profile_Firstname,
                Lastname = obj.Profile_Lastname,
            }
        };
    }

    #endregion

    #region GET /tasks/{id}/solution-attempt/run

    [HttpPost("tasks/{id}/solution-attempt/run")]
    public async Task<IActionResult> PostSolutionOfTaskAsync(
        Guid id, 
        SolutionCodeRequest request)
    {
        var userId = _identityService.GetUserIdentity();

        if (userId == null)
        {
            return NotFound("No user found");
        }

        var task = await _taskRepository.FindByIdAsync(id);

        if (task == null)
        {
            return NotFound("No task found");
        }

        var solutionValue = SolutionValue.Create(request.Code);
        if (solutionValue.IsFailure)
        {
            ModelState.AddModelError("Code", solutionValue.Error);
            return _apiBehaviorOptions.InvalidModelStateResponseFactory(ControllerContext);
        }

        var code = task.ExecutionCondition.ExecutionTemplate.Replace("{code}", request.Code);

        var execution = _executionGenerator.CreateExecution(code, (int)task.ExecutionCondition.TimeLimit.TotalMilliseconds, task.ProgrammingLanguage.Name);

        var result = await _executionCompiler.Execute(execution);

        var solution = new Solution(Guid.NewGuid(), id, Guid.Parse(userId), solutionValue.Value!, result.Success);

        _solutionRepository.Add(solution);

        await _solutionRepository.UnitOfWork.SaveChangesAsync();

        return Ok(new SolutionAttemptResultResponse
        {
            Id = solution.Id,
            Success = result.Success,
            Message = result.Success ? result.Value! : result.Error,
        });
    }

    #endregion
}
