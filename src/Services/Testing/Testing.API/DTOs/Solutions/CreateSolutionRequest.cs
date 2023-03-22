using System.ComponentModel.DataAnnotations;

namespace Testing.API.DTOs.Solutions;

public class CreateSolutionRequest
{
    public Guid TaskId { get; set; }
    [Required]
    public string SolutionValue { get; set; } = string.Empty;
}
