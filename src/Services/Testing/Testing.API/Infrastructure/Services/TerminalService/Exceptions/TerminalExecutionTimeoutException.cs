using System.Runtime.Serialization;

namespace Testing.API.Infrastructure.Services.TerminalService.Exceptions;

public class TerminalExecutionTimeoutException : Exception
{
    public TerminalExecutionTimeoutException()
    {
    }

    public TerminalExecutionTimeoutException(string? message) : base(message)
    {
    }

    public TerminalExecutionTimeoutException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected TerminalExecutionTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
