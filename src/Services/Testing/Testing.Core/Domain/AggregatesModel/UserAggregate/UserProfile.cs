using Common.Models.Base;

namespace Testing.Core.Domain.AggregatesModel.UserAggregate;

public record UserProfile
{
    public string Fullname { get; init; }

    private UserProfile(string fullname)
        => (Fullname) = (fullname);

    public static Result<UserProfile> Create(string fullname)
    {
        if (string.IsNullOrWhiteSpace(fullname))
            return Result.Fail<UserProfile>("Fullname can't be empty");

        if (fullname.Length > 128)
            return Result.Fail<UserProfile>("Fullname is too long");

        return Result.Ok(new UserProfile(fullname));
    }
}
