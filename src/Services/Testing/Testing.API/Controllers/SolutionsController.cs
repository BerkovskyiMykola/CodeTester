using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Testing.API.Application.Queries.Solutions;
using Testing.API.Application.Queries.Solutions.Models;
using Testing.API.DTOs.Solutions;
using Testing.API.Infrastructure.Services;
using Testing.Core.Domain.AggregatesModel.SolutionAggregate;
using Testing.Core.Domain.Repositories;

namespace Testing.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
[ApiController]
public class SolutionsController : ControllerBase
{
    private readonly ISolutionRepository _solutionRepository;
    private readonly ISolutionQueries _solutionQueries;
    private readonly IIdentityService _identityService;

    public SolutionsController(
        ISolutionRepository solutionRepository,
        ISolutionQueries solutionQueries,
        IIdentityService identityService)
    {
        _solutionRepository = solutionRepository;
        _solutionQueries = solutionQueries;
        _identityService = identityService;
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

    [HttpPut]
    public async Task<ActionResult<SolutionQueryModel>> UpsertSolutionAsync(UpsertSolutionRequest request)
    {
        var solutionValue = SolutionValue.Create(request.SolutionValue);
        if (solutionValue.IsFailure)
        {
            return BadRequest("Solution value is invalid");
        }

        var solution = new Solution(request.Id ?? Guid.Empty, request.TaskId, request.UserId, solutionValue.Value!, request.Success);
        solution = _solutionRepository.Upsert(solution);
        await _solutionRepository.UnitOfWork.SaveChangesAsync();

        return Ok(solution.Id);
    }
}
