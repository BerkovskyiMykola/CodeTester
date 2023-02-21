using System.ComponentModel.DataAnnotations;

namespace Testing.API.DTOs.Solutions;

public class UpsertSolutionRequest
{
    public Guid? Id { get; set; }

    public Guid UserId { get; set; }

    public Guid TaskId { get; set; }

    [Required]
    public string SolutionValue { get; set; } = string.Empty;

    [Required]
    public bool Success { get; set; }
}
