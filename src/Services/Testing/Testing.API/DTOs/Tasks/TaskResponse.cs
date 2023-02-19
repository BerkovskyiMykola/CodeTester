namespace Testing.API.DTOs.Tasks;

public class TaskResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string DescriptionText { get; set; } = string.Empty;
    public string DescriptionExamples { get; set; } = string.Empty;
    public string? DescriptionCases { get; set; }
    public string? DescriptionNote { get; set; }
    public int DifficultyId { get; set; }
    public string DifficultyName { get; set; } = string.Empty;
    public int TaskTypeId { get; set; }
    public string TaskTypeName { get; set; } = string.Empty;
    public int ProgrammingLanguageId { get; set; }
    public string ProgrammingLanguageName { get; set; } = string.Empty;
    public string? SolutionExampleDescription { get; set; }
    public string SolutionExample { get; set; } = string.Empty;
    public string ExecutionConditionTests { get; set; } = string.Empty;
    public TimeSpan ExecutionTimeLimit { get; set; }
    public DateTime CreateDate { get; set; }
}
