using Testing.Core.Bases;

namespace Testing.Core.Domain.AggregatesModel.TaskAggregate;

public record ProgrammingLanguage
{
    public int Id { get; init; }
    public string Name { get; init; }

    private ProgrammingLanguage(int id, string name)
        => (Id, Name) = (id, name);

    public static Result<ProgrammingLanguage> Create(int id, string name)
    {
        if (id <= 0)
            return Result.Fail<ProgrammingLanguage>("Id is less than one");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail<ProgrammingLanguage>("Name can't be empty");

        if (name.Length > 50)
            return Result.Fail<ProgrammingLanguage>("Name is too long");

        return Result.Ok(new ProgrammingLanguage(id, name));
    }
}
