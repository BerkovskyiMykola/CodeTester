using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTO.Requests;

public class ResetPasswordConfirmRequest
{
    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;
}
