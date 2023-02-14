using System.ComponentModel.DataAnnotations;

namespace Dictionary.API.DTOs.Difficulties;

public class UpdateDifficultyRequest
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
}
