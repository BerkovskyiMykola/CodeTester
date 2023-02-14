using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTOs.Requests;

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; init; } = string.Empty;

    [Required]
    public string NewPassword { get; init; } = string.Empty;
}
