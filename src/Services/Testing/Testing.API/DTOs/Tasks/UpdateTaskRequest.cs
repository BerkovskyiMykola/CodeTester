using System.ComponentModel.DataAnnotations;

namespace Testing.API.DTOs.Tasks;

public class UpdateTaskRequest
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string DescriptionText { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string DescriptionExamples { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? DescriptionCases { get; set; }

    [StringLength(1000)]
    public string? DescriptionNote { get; set; }

    [Range(1, int.MaxValue)]
    public int DifficultyId { get; set; }

    [Range(1, int.MaxValue)]
    public int TaskTypeId { get; set; }

    [Range(1, int.MaxValue)]
    public int ProgrammingLanguageId { get; set; }

    [StringLength(1000)]
    public string? SolutionExampleDescription { get; set; }

    [Required]
    public string SolutionExample { get; set; } = string.Empty;

    [Required]
    public string ExecutionConditionTests { get; set; } = string.Empty;

    [Required]
    public TimeSpan ExecutionTimeLimit { get; set; }
}
