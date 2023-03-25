namespace Testing.API.DTOs.Solutions;

public class SolutionAppemptResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool Success { get; set; }
    public DateTime CreateDate { get; set; }
}
