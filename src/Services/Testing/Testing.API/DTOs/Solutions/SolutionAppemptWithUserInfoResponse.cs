using Testing.API.DTOs.Users;

namespace Testing.API.DTOs.Solutions;

public class SolutionAppemptWithUserInfoResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool Success { get; set; }
    public DateTime CreateDate { get; set; }
    public UserResponse User { get; set; } = new UserResponse();
}
