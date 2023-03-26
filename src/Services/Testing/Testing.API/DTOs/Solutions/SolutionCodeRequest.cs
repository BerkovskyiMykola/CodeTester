using System.ComponentModel.DataAnnotations;

namespace Testing.API.DTOs.Solutions;

public class SolutionCodeRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;
}
