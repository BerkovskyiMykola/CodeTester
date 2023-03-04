using System.Runtime.Serialization;

namespace Testing.API.Infrastructure.Services.TerminalService.Exceptions;

public class CommandExecutionException : Exception
{
    public CommandExecutionException()
    {
    }

    public CommandExecutionException(string? message) : base(message)
    {
    }

    public CommandExecutionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CommandExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
