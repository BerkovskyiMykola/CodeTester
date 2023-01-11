using Core.Bases;

namespace Core.Domain.AggregatesModel.TaskAggregate;

public record Description
{
    public string Text { get; init; }
    public string Examples { get; init; }
    public string? SomeCases { get; init; }
    public string? Note { get; init; }

    private Description(string text, string examples, string? someCases, string? note)
        => (Text, Examples, SomeCases, Note) = (text, examples, someCases, note);

    public static Result<Description> Create(string text, string examples, string? someCases, string? note)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result.Fail<Description>("Text can't be empty");

        if (text.Length > 1000)
            return Result.Fail<Description>("Text is too long");

        if (string.IsNullOrWhiteSpace(examples))
            return Result.Fail<Description>("Examples can't be empty");

        if (examples.Length > 1000)
            return Result.Fail<Description>("Examples is too long");

        if (someCases is not null && someCases.Length > 1000)
            return Result.Fail<Description>("SomeCases is too long");

        if (note is not null && note.Length > 1000)
            return Result.Fail<Description>("Note is too long");

        return Result.Ok(new Description(text, examples, someCases, note));
    }
}
