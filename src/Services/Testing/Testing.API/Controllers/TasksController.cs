﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Testing.API.Application.Queries.Tasks;
using Testing.API.Application.Queries.Tasks.Models;
using Testing.API.DTOs.Tasks;
using Testing.API.Infrastructure.Services.DictionaryService;
using Testing.Core.Domain.AggregatesModel.TaskAggregate;
using Testing.Core.Domain.Repositories;
using DomainTask = Testing.Core.Domain.AggregatesModel.TaskAggregate.Task;
using DomainType = Testing.Core.Domain.AggregatesModel.TaskAggregate.Type;

namespace Testing.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly IDictionaryService _dictionaryService;
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskQueries _taskQueries;

    public TasksController(
        IDictionaryService dictionaryService,
        ITaskRepository taskRepository,
        ITaskQueries taskQueries)
    {
        _dictionaryService = dictionaryService;
        _taskRepository = taskRepository;
        _taskQueries = taskQueries;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskQueryModel>> GetTaskAsync(Guid taskId)
    {
        try
        {
            var task = await _taskQueries.GetTaskAsync(taskId);
            return Ok(task);
        }
        catch
        {
            return NotFound("Task not found");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskQueryModel>>> GetAllTasksAsync()
    {
        try
        {
            var tasks = await _taskQueries.GetAllTasksAsync();
            return Ok(tasks);
        }
        catch
        {
            return NotFound("Tasks not found");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TaskQueryModel>> CreateTaskAsync(CreateTaskRequest request)
    {
        var title = Title.Create(request.Title);
        if (title.IsFailure)
        {
            return BadRequest("Title is invalid");
        }

        var description = Description.Create(request.TaskDescription.Text, request.TaskDescription.Examples,
                                             request.TaskDescription.SomeCases, request.TaskDescription.Note);
        if (description.IsFailure)
        {
            return BadRequest("Description is invalid");
        }

        var difficultyData = await _dictionaryService.GetDifficultyByIdAsync(request.DifficultyId);
        if (difficultyData == null)
        {
            return NotFound("Difficulty is not found");
        }
        var difficulty = Difficulty.Create(difficultyData.Id, difficultyData.Name);

        var taskTypeData = await _dictionaryService.GetTaskTypeByIdAsync(request.TaskTypeId);
        if (taskTypeData == null)
        {
            return NotFound("TaskType is not found");
        }
        var taskType = DomainType.Create(taskTypeData.Id, taskTypeData.Name);

        var programmingLanguageData = await _dictionaryService.GetProgrammingLanguageByIdAsync(request.ProgrammingLanguageId);
        if (programmingLanguageData == null)
        {
            return NotFound("ProgrammingLanguage is not found");
        }
        var programmingLanguage = ProgrammingLanguage.Create(programmingLanguageData.Id, programmingLanguageData.Name);

        var solutionExample = SolutionExample.Create(request.TaskSolutionExample.Description, request.TaskSolutionExample.Solution);
        if (solutionExample.IsFailure)
        {
            return BadRequest("SolutionExample is invalid");
        }

        var executionCondition = ExecutionCondition.Create(request.TaskExecutionCondition.Tests, request.TaskExecutionCondition.TimeLimit);
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

        task = _taskRepository.Add(task);
        await _taskRepository.UnitOfWork.SaveChangesAsync();

        return Ok(task.Id);
    }

    [HttpPut]
    public async Task<ActionResult<TaskQueryModel>> UpdateTaskAsync(UpdateTaskRequest request)
    {
        var task = await _taskRepository.FindByIdAsync(request.Id);
        if (task == null)
        {
            return NotFound("Task to update not found");
        }

        var title = Title.Create(request.Title);
        if (title.IsFailure)
        {
            return BadRequest("Title is invalid");
        }

        var description = Description.Create(request.TaskDescription.Text, request.TaskDescription.Examples,
                                             request.TaskDescription.SomeCases, request.TaskDescription.Note);
        if (description.IsFailure)
        {
            return BadRequest("Description is invalid");
        }

        var difficultyData = await _dictionaryService.GetDifficultyByIdAsync(request.DifficultyId);
        if (difficultyData == null)
        {
            return NotFound("Difficulty is not found");
        }
        var difficulty = Difficulty.Create(difficultyData.Id, difficultyData.Name);

        var taskTypeData = await _dictionaryService.GetTaskTypeByIdAsync(request.TaskTypeId);
        if (taskTypeData == null)
        {
            return NotFound("TaskType is not found");
        }
        var taskType = DomainType.Create(taskTypeData.Id, taskTypeData.Name);

        var programmingLanguageData = await _dictionaryService.GetProgrammingLanguageByIdAsync(request.ProgrammingLanguageId);
        if (programmingLanguageData == null)
        {
            return NotFound("ProgrammingLanguage is not found");
        }
        var programmingLanguage = ProgrammingLanguage.Create(programmingLanguageData.Id, programmingLanguageData.Name);

        var solutionExample = SolutionExample.Create(request.TaskSolutionExample.Description, request.TaskSolutionExample.Solution);
        if (solutionExample.IsFailure)
        {
            return BadRequest("SolutionExample is invalid");
        }

        var executionCondition = ExecutionCondition.Create(request.TaskExecutionCondition.Tests, request.TaskExecutionCondition.TimeLimit);
        if (executionCondition.IsFailure)
        {
            return BadRequest("ExecutionCondition is invalid");
        }

        task.SetNewTitle(title.Value!);
        task.SetNewDescription(description.Value!);
        task.SetNewDifficulty(difficulty.Value!);
        task.SetNewType(taskType.Value!);
        task.SetNewProgrammingLanguage(programmingLanguage.Value!);
        task.SetNewSolutionExample(solutionExample.Value!);
        task.SetNewExecutionCondition(executionCondition.Value!);

        _taskRepository.Update(task);
        await _taskRepository.UnitOfWork.SaveChangesAsync();

        return Ok(task.Id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTaskAsync(Guid id)
    {
        var task = await _taskRepository.FindByIdAsync(id);
        if (task == null)
        {
            return NotFound("Task to delete not found");
        }

        _taskRepository.Delete(task);
        await _taskRepository.UnitOfWork.SaveChangesAsync();

        return NoContent();
    }
}