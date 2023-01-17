using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTO.Requests;

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; init; } = string.Empty;

    [Required]
    public string NewPassword { get; init; } = string.Empty;
}
