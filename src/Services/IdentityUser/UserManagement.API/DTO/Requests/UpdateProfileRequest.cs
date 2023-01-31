using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTO.Requests;

public class UpdateProfileRequest
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; init; } = string.Empty;
}
