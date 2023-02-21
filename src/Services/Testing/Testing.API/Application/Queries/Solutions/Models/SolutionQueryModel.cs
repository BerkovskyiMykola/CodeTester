namespace Testing.API.Application.Queries.Solutions.Models;

public class SolutionQueryModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TaskId { get; set; }
    public string SolutionValue { get; set; } = string.Empty;
    public bool Success { get; set; }
}
