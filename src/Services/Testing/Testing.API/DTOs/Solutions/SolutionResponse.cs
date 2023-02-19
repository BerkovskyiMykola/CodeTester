namespace Testing.API.DTOs.Solutions;

public class SolutionResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TaskId { get; set; }
    public string SolutionValue { get; set; } = string.Empty;
    public bool Success { get; set; }
}
