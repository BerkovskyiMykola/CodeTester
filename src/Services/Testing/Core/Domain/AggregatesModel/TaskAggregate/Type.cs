using Core.Bases;

namespace Core.Domain.AggregatesModel.TaskAggregate;

public record Type
{
    public int Id { get; init; }
    public string Name { get; init; }

    private Type(int id, string name)
        => (Id, Name) = (id, name);

    public static Result<Type> Create(int id, string name)
    {
        if (id <= 0)
            return Result.Fail<Type>("Id is less than one");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail<Type>("Name can't be empty");

        if (name.Length > 50)
            return Result.Fail<Type>("Name is too long");

        return Result.Ok(new Type(id, name));
    }
}
