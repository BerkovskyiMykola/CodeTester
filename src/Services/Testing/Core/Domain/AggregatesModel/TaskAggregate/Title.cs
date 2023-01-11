using Core.Bases;

namespace Core.Domain.AggregatesModel.TaskAggregate;

public record Title
{
    public string Value { get; init; }

    private Title(string value)
        => Value = value;

    public static Result<Title> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Fail<Title>("Title can't be empty");

        if (title.Length > 100)
            return Result.Fail<Title>("Title is too long");

        return Result.Ok(new Title(title));
    }
}
