using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Testing.API.Application.Queries.Solutions;
using Testing.API.Application.Queries.Solutions.Models;
using Testing.API.DTOs.Solutions;
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

    public SolutionsController(
        ISolutionRepository solutionRepository,
        ISolutionQueries solutionQueries)
    {
        _solutionRepository = solutionRepository;
        _solutionQueries = solutionQueries;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SolutionQueryModel>> GetSolutionAsync(Guid solutionId)
    {
        try
        {
            var solution = await _solutionQueries.GetSolutionAsync(solutionId);
            return Ok(solution);
        }
        catch
        {
            return NotFound("Solution not found");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SolutionQueryModel>>> GetAllSolutionsAsync()
    {
        try
        {
            var solutions = await _solutionQueries.GetAllSolutionsAsync();
            return Ok(solutions);
        }
        catch
        {
            return NotFound("Solutions not found");
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
