namespace Testing.API.DTOs.Tasks;

public class TaskDescriptionResponse
{
    public string Text { get; set; } = string.Empty;
    public string Examples { get; set; } = string.Empty;
    public string? SomeCases { get; set; }
    public string? Note { get; set; }
}
