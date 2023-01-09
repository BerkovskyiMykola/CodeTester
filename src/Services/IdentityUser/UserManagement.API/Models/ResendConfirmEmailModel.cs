using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.Models;

public class ResendConfirmEmailModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
