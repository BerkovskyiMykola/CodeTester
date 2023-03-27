using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities;

public class ApplicationUser : IdentityUser
{
    public string Fullname { get; set; } = string.Empty;
    public string? PhotoFileName { get; set; }
}
