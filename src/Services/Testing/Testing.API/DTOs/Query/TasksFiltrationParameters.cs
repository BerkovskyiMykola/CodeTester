namespace Testing.API.DTOs.Query;

public class TasksFiltrationParameters
{
    public string? Search { get; set; }
    public int? DifficultyId { get; set; }
    public int? ProgrammingLanguageId { get; set; }
    public int? TypeId { get; set; }
    public bool? IsCompleted { get; set; }
}