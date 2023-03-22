using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Testing.API.Application.Queries.Solutions;
using Testing.API.Application.Queries.Solutions.Models;
using Testing.API.DTOs.Solutions;
using Testing.API.Infrastructure.Services;
using Testing.API.Infrastructure.Services.ExecutionCompiler;
using Testing.API.Infrastructure.Services.ExecutionGenerator;
using Testing.Core.Domain.AggregatesModel.SolutionAggregate;
using Testing.Core.Domain.Repositories;

namespace Testing.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
[ApiController]
public class SolutionsController : ControllerBase
{
    private readonly ISolutionRepository _solutionRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly ISolutionQueries _solutionQueries;
    private readonly IIdentityService _identityService;
    private readonly IExecutionCompiler _executionCompiler;
    private readonly IExecutionGenerator _executionGenerator;

    public SolutionsController(
        ISolutionRepository solutionRepository,
        ISolutionQueries solutionQueries,
        IIdentityService identityService,
        ITaskRepository taskRepository,
        IExecutionGenerator executionGenerator,
        IExecutionCompiler executionCompiler)
    {
        _solutionRepository = solutionRepository;
        _solutionQueries = solutionQueries;
        _identityService = identityService;
        _taskRepository = taskRepository;
        _executionCompiler = executionCompiler;
        _executionGenerator = executionGenerator;
    }

    [HttpGet("solution/task/{taskId}")]
    public async Task<ActionResult<SolutionQueryModel>> GetSolutionAsync(Guid taskId)
    {
        var userId = _identityService.GetUserIdentity();

        if (userId == null)
        {
            return NotFound("No user found");
        }

        try
        {
            var solution = await _solutionQueries.GetSolutionByUserIdAndTaskIdAsync(Guid.Parse(userId), taskId);
            return Ok(solution);
        }
        catch
        {
            return NotFound("Solution not found");
        }
    }

    [HttpPost]
    public async Task<ActionResult<SolutionQueryModel>> PostSolutionAsync(CreateSolutionRequest request)
    {
        var userId = _identityService.GetUserIdentity();

        if (userId == null)
        {
            return NotFound("No user found");
        }

        var task = await _taskRepository.FindByIdAsync(request.TaskId);

        if (task == null)
        {
            return NotFound("No task found");
        }

        var solutionValue = SolutionValue.Create(request.SolutionValue);
        if (solutionValue.IsFailure)
        {
            return BadRequest("Solution value is invalid");
        }

        var code = task.ExecutionCondition.ExecutionTemplate.Replace("{code}", request.SolutionValue);

        var execution = _executionGenerator.CreateExecution(code, (int)task.ExecutionCondition.TimeLimit.TotalMilliseconds, task.ProgrammingLanguage.Name);
        
        var result = await _executionCompiler.Execute(execution);

        var solution = new Solution(Guid.NewGuid(), request.TaskId, Guid.Parse(userId), solutionValue.Value!, result.Success);
        _solutionRepository.Add(solution);
        await _solutionRepository.UnitOfWork.SaveChangesAsync();

        return Ok(new
        {
            solution.Id,
            result.Success,
            Message = result.Success ? result.Value : result.Error,
        });
    }
}
