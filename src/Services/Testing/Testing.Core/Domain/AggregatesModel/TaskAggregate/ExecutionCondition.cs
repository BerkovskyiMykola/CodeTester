using Testing.Core.Bases;

namespace Testing.Core.Domain.AggregatesModel.TaskAggregate;

public record ExecutionCondition
{
    public string ExecutionTemplate { get; init; }
    public TimeSpan TimeLimit { get; init; }

    private ExecutionCondition(string executionTemplate, TimeSpan timeLimit)
        => (ExecutionTemplate, TimeLimit) = (executionTemplate, timeLimit);

    public static Result<ExecutionCondition> Create(string executionTemplate, TimeSpan timeLimit)
    {
        if (string.IsNullOrWhiteSpace(executionTemplate))
            return Result.Fail<ExecutionCondition>("ExecutionTemplate can't be empty");

        if (!executionTemplate.Contains("{code}"))
            return Result.Fail<ExecutionCondition>("ExecutionTemplate doesn't contain '{code}'");

        if (timeLimit.TotalSeconds < 1)
            return Result.Fail<ExecutionCondition>("TimeLimit can't be less than 1 second");

        return Result.Ok(new ExecutionCondition(executionTemplate, timeLimit));
    }
}
