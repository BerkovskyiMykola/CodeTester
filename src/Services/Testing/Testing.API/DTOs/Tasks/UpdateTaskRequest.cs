using System.ComponentModel.DataAnnotations;

namespace Testing.API.DTOs.Tasks;

public class UpdateTaskRequest
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    public TaskDescriptionRequest TaskDescription { get; set; } = new();

    public int DifficultyId { get; set; }

    public int TaskTypeId { get; set; }

    public int ProgrammingLanguageId { get; set; }

    public TaskSolutionTemplate TaskSolutionTemplate { get; set; } = new();
    public TaskExecitonConditionRequest TaskExecutionCondition { get; set; } = new();
}
