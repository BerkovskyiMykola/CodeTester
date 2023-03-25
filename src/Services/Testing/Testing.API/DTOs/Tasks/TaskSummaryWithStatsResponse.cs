using Testing.API.DTOs.Base;

namespace Testing.API.DTOs.Tasks;

public class TaskSummaryWithStatsResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DifficultyResponse Difficulty { get; set; } = new DifficultyResponse();
    public TaskTypeResponse TaskType { get; set; } = new TaskTypeResponse();
    public ProgrammingLanguageResponse ProgrammingLanguage { get; set; } = new ProgrammingLanguageResponse();
    public long CompletedAmount { get; set; }
    public bool IsCompleted { get; set; }
}
