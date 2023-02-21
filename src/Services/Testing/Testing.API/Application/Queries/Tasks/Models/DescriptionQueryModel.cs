namespace Testing.API.Application.Queries.Tasks.Models;

public class DescriptionQueryModel
{
    public string Text { get; set; } = string.Empty;
    public string Examples { get; set; } = string.Empty;
    public string? SomeCases { get; set; }
    public string? Note { get; set; }
}
