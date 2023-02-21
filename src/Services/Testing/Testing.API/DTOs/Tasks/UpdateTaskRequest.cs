using System.ComponentModel.DataAnnotations;

namespace Testing.API.DTOs.Tasks;

public class UpdateTaskRequest
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    public UpdateTaskRequestDescription TaskDescription { get; set; } = new();

    public int DifficultyId { get; set; }

    public int TaskTypeId { get; set; }

    public int ProgrammingLanguageId { get; set; }

    public UpdateTaskRequestSolutionExample TaskSolutionExample { get; set; } = new();
    public UpdateTaskRequestExecitonCondition TaskExecutionCondition { get; set; } = new();
}

public class UpdateTaskRequestDescription
{
    [Required]
    [StringLength(1000)]
    public string Text { get; set; } = string.Empty;
    [Required]
    [StringLength(1000)]
    public string Examples { get; set; } = string.Empty;
    [StringLength(1000)]
    public string? SomeCases { get; set; }
    [StringLength(1000)]
    public string? Note { get; set; }
}

public class UpdateTaskRequestSolutionExample
{
    [StringLength(1000)]
    public string? Description { get; set; }
    [Required]
    public string Solution { get; set; } = string.Empty;
}
public class UpdateTaskRequestExecitonCondition
{
    [Required]
    public string Tests { get; set; } = string.Empty;
    [Required]
    [Range(typeof(TimeSpan), "00:00:01", "23:59")]
    public TimeSpan TimeLimit { get; set; }
}