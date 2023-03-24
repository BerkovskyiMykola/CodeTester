using Common.Models.Base;

namespace Testing.Core.Domain.AggregatesModel.TaskAggregate;

public record SolutionTemplate
{
    public string Value { get; init; }

    private SolutionTemplate(string value)
        => (Value) = (value);

    public static Result<SolutionTemplate> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail<SolutionTemplate>("SolutionTemplate can't be empty");

        return Result.Ok(new SolutionTemplate(value));
    }
}
