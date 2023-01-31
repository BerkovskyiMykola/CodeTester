using Testing.Core.Bases;

namespace Testing.Core.Domain.AggregatesModel.TaskAggregate;

public record SolutionExample
{
    public string? Description { get; init; }
    public string Solution { get; init; }

    private SolutionExample(string? description, string solution)
        => (Description, Solution) = (description, solution);

    public static Result<SolutionExample> Create(string? description, string solution)
    {
        if (description is not null && description.Length > 1000)
            return Result.Fail<SolutionExample>("Description is too long");

        if (string.IsNullOrWhiteSpace(solution))
            return Result.Fail<SolutionExample>("Solution can't be empty");

        return Result.Ok(new SolutionExample(description, solution));
    }
}
