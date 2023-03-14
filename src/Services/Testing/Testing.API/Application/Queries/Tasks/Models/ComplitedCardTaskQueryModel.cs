namespace Testing.API.Application.Queries.Tasks.Models;

public class ComplitedCardTaskQueryModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DifficultyQueryModel Difficulty { get; set; } = new DifficultyQueryModel();
    public TaskTypeQueryModel TaskType { get; set; } = new TaskTypeQueryModel();
    public ProgrammingLanguageQueryModel ProgrammingLanguage { get; set; } = new ProgrammingLanguageQueryModel();
    public long ComplitedAmount { get; set; }
    public bool IsComplited { get; set; }
}
