using System.ComponentModel.DataAnnotations;

namespace Testing.API.DTOs.Solutions;

public class UpsertSolutionRequest
{
    public Guid? Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid TaskId { get; set; }

    [Required]
    public string SolutionValue { get; set; } = string.Empty;

    [Required]
    public bool Success { get; set; }
}
