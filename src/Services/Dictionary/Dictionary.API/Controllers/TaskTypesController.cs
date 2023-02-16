using Dictionary.API.DTOs.TaskTypes;
using Dictionary.API.Extensions;
using Dictionary.API.Infrastructure;
using Dictionary.API.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Dictionary.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class TaskTypesController : ControllerBase
{
    private readonly DictionaryContext _context;

    public TaskTypesController(DictionaryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskTypeResponse>>> GetTaskTypes()
    {
        return await _context.TaskTypes
            .MapToTaskTypeResponse()
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskTypeResponse>> GetTaskType(int id)
    {
        var taskType = await _context.TaskTypes
            .MapToTaskTypeResponse()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (taskType == null)
        {
            return NotFound();
        }

        return taskType;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutTaskType(int id, UpdateTaskTypeRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }

        if (await _context.TaskTypes
            .Where(x => x.Id != id)
            .AnyAsync(x => x.Name.Trim().ToLower() == request.Name.Trim().ToLower()))
        {
            return BadRequest("The specified name already exists");
        }

        var taskType = await _context.TaskTypes.FirstOrDefaultAsync(x => x.Id == id);
        if (taskType == null)
        {
            return NotFound();
        }

        taskType.SetNewName(request.Name);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!TaskTypeExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> PostTaskType(CreateTaskTypeRequest request)
    {
        if (await _context.TaskTypes
            .AnyAsync(x => x.Name.Trim().ToLower() == request.Name.Trim().ToLower()))
        {
            return BadRequest("The specified name already exists");
        }

        var taskType = new TaskType(request.Name);

        _context.TaskTypes.Add(taskType);
        await _context.SaveChangesAsync();

        return Ok(taskType.Id);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTaskType(int id)
    {
        var taskType = await _context.TaskTypes.FirstOrDefaultAsync(x => x.Id == id);
        if (taskType == null)
        {
            return NotFound();
        }

        _context.TaskTypes.Remove(taskType);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TaskTypeExists(int id)
    {
        return _context.TaskTypes.Any(e => e.Id == id);
    }
}
