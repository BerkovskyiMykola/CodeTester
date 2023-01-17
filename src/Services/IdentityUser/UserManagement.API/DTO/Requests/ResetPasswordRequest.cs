using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTO.Requests;

public class ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
