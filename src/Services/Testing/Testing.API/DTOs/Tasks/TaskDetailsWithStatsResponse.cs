using Testing.API.DTOs.Base;

namespace Testing.API.DTOs.Tasks;

public class TaskDetailsWithStatsResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskDescriptionResponse Description { get; set; } = new TaskDescriptionResponse();
    public DifficultyResponse Difficulty { get; set; } = new DifficultyResponse();
    public TaskTypeResponse TaskType { get; set; } = new TaskTypeResponse();
    public ProgrammingLanguageResponse ProgrammingLanguage { get; set; } = new ProgrammingLanguageResponse();
    public string SolutionTemplate { get; set; } = string.Empty;
    public long CompletedAmount { get; set; }
    public bool IsCompleted { get; set; }
}
