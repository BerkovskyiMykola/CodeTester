using Core.Bases;

namespace Core.Domain.AggregatesModel.SolutionAggregate;

public record SolutionValue
{
    public string Value { get; init; }

    private SolutionValue(string value) => Value = value;

    public static Result<SolutionValue> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail<SolutionValue>("Value can't be empty");

        return Result.Ok(new SolutionValue(value));
    }
}