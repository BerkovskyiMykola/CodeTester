using System.ComponentModel.DataAnnotations;

namespace Testing.API.DTOs.Tasks;

public class TaskSolutionTemplate
{
    [Required]
    [StringLength(1000)]
    public string Value { get; set; } = string.Empty;
}
