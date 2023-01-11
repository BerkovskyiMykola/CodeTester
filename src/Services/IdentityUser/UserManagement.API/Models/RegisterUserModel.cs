using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.Models;

public record RegisterUserModel
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
