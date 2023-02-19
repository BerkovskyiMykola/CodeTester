using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Testing.API.Application.Queries.Solutions;
using Testing.API.DTOs.Solutions;
using Testing.API.Services;
using Testing.Core.Domain.AggregatesModel.SolutionAggregate;
using Testing.Core.Domain.Repositories;

namespace Testing.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
[ApiController]
public class SolutionController : Controller
{
    private readonly IDictionaryService _dictionary;
    private readonly ISolutionRepository _solutions;
    private readonly ISolutionQueries _solutionQueries;

    public SolutionController(
        IDictionaryService dictionaryService,
        ISolutionRepository solutionRepository,
        ISolutionQueries solutionQueries)
    {
        _dictionary = dictionaryService;
        _solutions = solutionRepository;
        _solutionQueries = solutionQueries;
    }

    [HttpGet("{solutionId}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(SolutionResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<SolutionResponse>> GetSolutionAsync(string solutionId)
    {
        try
        {
            var solution = await _solutionQueries.GetSolutionAsync(new Guid(solutionId));
            return Ok(solution);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(SolutionResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<SolutionResponse>>> GetAllSolutionsAsync()
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
    public async Task<ActionResult<SolutionResponse>> UpsertSolutionAsync([FromBody] UpsertSolutionRequest request)
    {
        var solutionValue = SolutionValue.Create(request.SolutionValue);
        if (solutionValue.IsFailure)
        {
            return BadRequest("Solution value is invalid");
        }

        var solution = new Solution(request.Id ?? Guid.Empty, request.TaskId, request.UserId, solutionValue.Value!, request.Success);
        solution = _solutions.Upsert(solution);
        await _solutions.UnitOfWork.SaveChangesAsync();

        return Ok(solution.Id);
    }
}
