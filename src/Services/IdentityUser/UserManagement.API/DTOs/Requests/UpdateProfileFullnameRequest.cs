using System.ComponentModel.DataAnnotations;

namespace UserManagement.API.DTOs.Requests;

public class UpdateProfileFullnameRequest
{
    [Required]
    [StringLength(128)]
    public string Fullname { get; init; } = string.Empty;
}
