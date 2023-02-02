using Testing.Core.Bases;

namespace Testing.Core.Domain.AggregatesModel.TaskAggregate;

public record Difficulty
{
    public int Id { get; init; }
    public string Name { get; init; }

    private Difficulty(int id, string name)
        => (Id, Name) = (id, name);

    public static Result<Difficulty> Create(int id, string name)
    {
        if (id <= 0)
            return Result.Fail<Difficulty>("Id is less than one");

        if (name.Length > 50)
            return Result.Fail<Difficulty>("Name is too long");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail<Difficulty>("Name can't be empty");

        if (name.Length > 50)
            return Result.Fail<Difficulty>("Name is too long");

        return Result.Ok(new Difficulty(id, name));
    }
}
