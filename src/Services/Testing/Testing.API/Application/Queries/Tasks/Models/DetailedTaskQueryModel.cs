namespace Testing.API.Application.Queries.Tasks.Models;

public class DetailedTaskQueryModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public DescriptionQueryModel Description { get; set; } = new();
    public DifficultyQueryModel Difficulty { get; set; } = new();
    public TaskTypeQueryModel TaskType { get; set; } = new();
    public ProgrammingLanguageQueryModel ProgrammingLanguage { get; set; } = new();
}
