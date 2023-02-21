using System.ComponentModel.DataAnnotations;

namespace Testing.API.DTOs.Tasks;

public class TaskDescriptionRequest
{
    [Required]
    [StringLength(1000)]
    public string Text { get; set; } = string.Empty;
    [Required]
    [StringLength(1000)]
    public string Examples { get; set; } = string.Empty;
    [StringLength(1000)]
    public string? SomeCases { get; set; }
    [StringLength(1000)]
    public string? Note { get; set; }
}
