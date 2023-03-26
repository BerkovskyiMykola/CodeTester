namespace Testing.API.DTOs.Solutions;

public class SolutionAttemptResultResponse
{
    public Guid Id { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
