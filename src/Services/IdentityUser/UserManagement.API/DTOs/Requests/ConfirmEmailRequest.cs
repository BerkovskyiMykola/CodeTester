using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTOs.Requests;

public class ConfirmEmailRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Token { get; set; } = string.Empty;
}
