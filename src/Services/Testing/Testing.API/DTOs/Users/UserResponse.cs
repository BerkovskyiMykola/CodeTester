namespace Testing.API.DTOs.Users;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Lastname { get; set; } = string.Empty;
    public string Firstname { get; set; } = string.Empty;
}
