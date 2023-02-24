using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTOs.Requests;

public class ConfirmEmailRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    [Required]
    public string Token { get; set; } = string.Empty;
}
