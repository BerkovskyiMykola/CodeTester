using System.ComponentModel.DataAnnotations;

namespace Testing.API.DTOs.Tasks;

public class CreateTaskRequest
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    public CreateTaskRequestDescription TaskDescription { get; set; } = new();

    public int DifficultyId { get; set; }

    public int TaskTypeId { get; set; }

    public int ProgrammingLanguageId { get; set; }

    public CreateTaskRequestSolutionExample TaskSolutionExample { get; set; } = new();
    public CreateTaskRequestExecitonCondition TaskExecutionCondition { get; set; } = new();
}

public class CreateTaskRequestDescription
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

public class CreateTaskRequestSolutionExample
{
    [StringLength(1000)]
    public string? Description { get; set; }
    [Required]
    public string Solution { get; set; } = string.Empty;
}
public class CreateTaskRequestExecitonCondition
{
    [Required]
    public string Tests { get; set; } = string.Empty;
    [Required]
    [Range(typeof(TimeSpan), "00:00:01", "23:59")]
    public TimeSpan TimeLimit { get; set; }
}