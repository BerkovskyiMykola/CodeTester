using Common.Models.Base;

namespace Testing.Core.Domain.AggregatesModel.UserAggregate;

public record UserProfile
{
    public string Lastname { get; init; }
    public string Firstname { get; init; }

    private UserProfile(string lastname, string firstname)
        => (Lastname, Firstname) = (lastname, firstname);

    public static Result<UserProfile> Create(string lastname, string firstname)
    {
        if (string.IsNullOrWhiteSpace(lastname))
            return Result.Fail<UserProfile>("Lastname can't be empty");

        if (lastname.Length > 50)
            return Result.Fail<UserProfile>("Lastname is too long");

        if (string.IsNullOrWhiteSpace(firstname))
            return Result.Fail<UserProfile>("Firstname can't be empty");

        if (firstname.Length > 50)
            return Result.Fail<UserProfile>("Firstname is too long");


        return Result.Ok(new UserProfile(lastname, firstname));
    }
}
