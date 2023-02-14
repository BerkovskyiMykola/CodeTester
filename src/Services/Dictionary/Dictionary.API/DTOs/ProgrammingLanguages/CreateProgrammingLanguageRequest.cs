using System.ComponentModel.DataAnnotations;

namespace Dictionary.API.DTOs.ProgrammingLanguages;

public class CreateProgrammingLanguageRequest
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
}
