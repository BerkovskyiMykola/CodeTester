using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.Models;

public class ConfirmEmailModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Token { get; set; } = string.Empty;
}
