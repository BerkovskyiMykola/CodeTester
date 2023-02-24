using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTOs.Requests;

public record RegisterUserRequest
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{6,}$")]
    public string Password { get; init; } = string.Empty;
}
