using Testing.Core.Bases;

namespace Testing.Core.Domain.AggregatesModel.TaskAggregate;

public record ExecutionCondition
{
    public string Tests { get; init; }
    public TimeSpan TimeLimit { get; init; }

    private ExecutionCondition(string tests, TimeSpan timeLimit)
        => (Tests, TimeLimit) = (tests, timeLimit);

    public static Result<ExecutionCondition> Create(string tests, TimeSpan timeLimit)
    {
        if (string.IsNullOrWhiteSpace(tests))
            return Result.Fail<ExecutionCondition>("Tests can't be empty");

        if (timeLimit.TotalSeconds < 1)
            return Result.Fail<ExecutionCondition>("TimeLimit can't be less than 1 second");

        return Result.Ok(new ExecutionCondition(tests, timeLimit));
    }
}
