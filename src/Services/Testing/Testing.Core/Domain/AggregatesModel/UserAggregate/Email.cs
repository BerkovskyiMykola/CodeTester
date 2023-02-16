using Testing.Core.Bases;

namespace Testing.Core.Domain.AggregatesModel.UserAggregate;

public record Email
{
    public string Value { get; init; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail<Email>("Email can't be empty");

        if (!IsValidEmail(value))
            return Result.Fail<Email>("Email is invalid");

        return Result.Ok(new Email(value));
    }

    private static bool IsValidEmail(string value)
    {
        if (value.Length == 0)
        {
            return false;
        }

        int index = value.IndexOf('@');

        return
            index > 0 &&
            index != value.Length - 1 &&
            index == value.LastIndexOf('@');
    }
}
