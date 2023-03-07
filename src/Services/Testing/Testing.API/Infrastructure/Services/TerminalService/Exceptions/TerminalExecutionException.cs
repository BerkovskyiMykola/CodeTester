using System.Runtime.Serialization;

namespace Testing.API.Infrastructure.Services.TerminalService.Exceptions;

public class TerminalExecutionException : Exception
{
    public TerminalExecutionException()
    {
    }

    public TerminalExecutionException(string? message) : base(message)
    {
    }

    public TerminalExecutionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected TerminalExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
