using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTOs.Requests;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
