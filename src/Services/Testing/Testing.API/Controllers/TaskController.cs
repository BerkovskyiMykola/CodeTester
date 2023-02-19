using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Testing.API.Application.Queries.Tasks;
using Testing.API.DTOs.Tasks;
using Testing.API.Services;
using Testing.Core.Domain.AggregatesModel.TaskAggregate;
using Testing.Core.Domain.Repositories;
using DomainTask = Testing.Core.Domain.AggregatesModel.TaskAggregate.Task;
using DomainType = Testing.Core.Domain.AggregatesModel.TaskAggregate.Type;

namespace Testing.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
[ApiController]
public class TaskController : Controller
{
    private readonly IDictionaryService _dictionary;
    private readonly ITaskRepository _tasks;
    private readonly ITaskQueries _taskQueries;

    public TaskController(
        IDictionaryService dictionaryService,
        ITaskRepository taskRepository,
        ITaskQueries taskQueries)
    {
        _dictionary = dictionaryService;
        _tasks = taskRepository;
        _taskQueries = taskQueries;
    }

    [HttpGet("{taskId}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(TaskResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<TaskResponse>> GetTaskAsync(string taskId)
    {
        try
        {
            var task = await _taskQueries.GetTaskAsync(new Guid(taskId));
            return Ok(task);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(TaskResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetAllTasksAsync()
    {
        try
        {
            var tasks = await _taskQueries.GetAllTasksAsync();
            return Ok(tasks);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(TaskResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<TaskResponse>> CreateTaskAsync([FromBody] CreateTaskRequest request)
    {
        var title = Title.Create(request.Title);
        if (title.IsFailure)
        {
            return BadRequest("Title is invalid");
        }

        var description = Description.Create(request.DescriptionText, request.DescriptionExamples,
                                             request.DescriptionCases, request.DescriptionNote);
        if (description.IsFailure)
        {
            return BadRequest("Description is invalid");
        }

        var difficultyData = await _dictionary.GetDifficultyByIdAsync(request.DifficultyId);
        if (difficultyData == null)
        {
            return BadRequest("Difficulty is invalid");
        }
        var difficulty = Difficulty.Create(difficultyData.Id, difficultyData.Name);

        var taskTypeData = await _dictionary.GetTaskTypeByIdAsync(request.TaskTypeId);
        if (taskTypeData == null)
        {
            return BadRequest("TaskType is invalid");
        }
        var taskType = DomainType.Create(taskTypeData.Id, taskTypeData.Name);

        var programmingLanguageData = await _dictionary.GetProgrammingLanguageByIdAsync(request.ProgrammingLanguageId);
        if (programmingLanguageData == null)
        {
            return BadRequest("ProgrammingLanguage is invalid");
        }
        var programmingLanguage = ProgrammingLanguage.Create(programmingLanguageData.Id, programmingLanguageData.Name);

        var solutionExample = SolutionExample.Create(request.SolutionExampleDescription, request.SolutionExample);
        if (solutionExample.IsFailure)
        {
            return BadRequest("SolutionExample is invalid");
        }

        var executionCondition = ExecutionCondition.Create(request.ExecutionConditionTests, request.ExecutionTimeLimit);
        if (executionCondition.IsFailure)
        {
            return BadRequest("ExecutionCondition is invalid");
        }

        var task = new DomainTask(
            Guid.Empty,
            title.Value!,
            description.Value!,
            difficulty.Value!,
            taskType.Value!,
            programmingLanguage.Value!,
            solutionExample.Value!,
            executionCondition.Value!
            );

        task = _tasks.Add(task);
        await _tasks.UnitOfWork.SaveChangesAsync();

        return Ok(task.Id);
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(TaskResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<TaskResponse>> UpdateTaskAsync([FromBody] UpdateTaskRequest request)
    {
        if (request.Id == null)
        {
            return NotFound();
        }
        Guid id = new Guid(request.Id.ToString()!);

        var title = Title.Create(request.Title);
        if (title.IsFailure)
        {
            return BadRequest("Title is invalid");
        }

        var description = Description.Create(request.DescriptionText, request.DescriptionExamples,
                                             request.DescriptionCases, request.DescriptionNote);
        if (description.IsFailure)
        {
            return BadRequest("Description is invalid");
        }

        var difficultyData = await _dictionary.GetDifficultyByIdAsync(request.DifficultyId);
        if (difficultyData == null)
        {
            return BadRequest("Difficulty is invalid");
        }
        var difficulty = Difficulty.Create(difficultyData.Id, difficultyData.Name);

        var taskTypeData = await _dictionary.GetTaskTypeByIdAsync(request.TaskTypeId);
        if (taskTypeData == null)
        {
            return BadRequest("TaskType is invalid");
        }
        var taskType = DomainType.Create(taskTypeData.Id, taskTypeData.Name);

        var programmingLanguageData = await _dictionary.GetProgrammingLanguageByIdAsync(request.ProgrammingLanguageId);
        if (programmingLanguageData == null)
        {
            return BadRequest("ProgrammingLanguage is invalid");
        }
        var programmingLanguage = ProgrammingLanguage.Create(programmingLanguageData.Id, programmingLanguageData.Name);

        var solutionExample = SolutionExample.Create(request.SolutionExampleDescription, request.SolutionExample);
        if (solutionExample.IsFailure)
        {
            return BadRequest("SolutionExample is invalid");
        }

        var executionCondition = ExecutionCondition.Create(request.ExecutionConditionTests, request.ExecutionTimeLimit);
        if (executionCondition.IsFailure)
        {
            return BadRequest("ExecutionCondition is invalid");
        }

        var task = new DomainTask(
            id,
            title.Value!,
            description.Value!,
            difficulty.Value!,
            taskType.Value!,
            programmingLanguage.Value!,
            solutionExample.Value!,
            executionCondition.Value!
            );

        task = _tasks.Update(task);
        await _tasks.UnitOfWork.SaveChangesAsync();

        return Ok(task.Id);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteTaskAsync(string id)
    {
        try
        {
            var task = await _taskQueries.GetTaskAsync(new Guid(id));
            await _tasks.Delete(task.Id);
            await _tasks.UnitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch
        {
            return NotFound();
        }
    }
}
