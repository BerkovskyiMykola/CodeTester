using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Testing.API.Application.Queries.Solutions;
using Testing.API.Application.Queries.Solutions.Models;
using Testing.API.DTOs.Solutions;
using Testing.Core.Domain.AggregatesModel.SolutionAggregate;
using Testing.Core.Domain.Repositories;

namespace Testing.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
[ApiController]
public class SolutionController : Controller
{
    private readonly ISolutionRepository _solutionRepository;
    private readonly ISolutionQueries _solutionQueries;

    public SolutionController(
        ISolutionRepository solutionRepository,
        ISolutionQueries solutionQueries)
    {
        _solutionRepository = solutionRepository;
        _solutionQueries = solutionQueries;
    }

    [HttpGet("{solutionId}")]
    public async Task<ActionResult<SolutionQueriesModel>> GetSolutionAsync(Guid solutionId)
    {
        try
        {
            var solution = await _solutionQueries.GetSolutionAsync(solutionId);
            return Ok(solution);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SolutionQueriesModel>>> GetAllSolutionsAsync()
    {
        try
        {
            var solutions = await _solutionQueries.GetAllSolutionsAsync();
            return Ok(solutions);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPut]
    public async Task<ActionResult<SolutionQueriesModel>> UpsertSolutionAsync(UpsertSolutionRequest request)
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
