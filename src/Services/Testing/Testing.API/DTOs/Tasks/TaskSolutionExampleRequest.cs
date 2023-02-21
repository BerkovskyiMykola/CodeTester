using System.ComponentModel.DataAnnotations;

namespace Testing.API.DTOs.Tasks;

public class TaskSolutionExampleRequest
{
    [StringLength(1000)]
    public string? Description { get; set; }
    [Required]
    public string Solution { get; set; } = string.Empty;
}
