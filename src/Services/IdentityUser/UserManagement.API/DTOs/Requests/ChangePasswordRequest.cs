using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTOs.Requests;

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; init; } = string.Empty;

    [Required]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{6,}$")]
    public string NewPassword { get; init; } = string.Empty;
}
