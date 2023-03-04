using System.Runtime.Serialization;

namespace Testing.API.Infrastructure.Services.TerminalService.Exceptions;

public class ProcessExecutionTimeoutException : Exception
{
    public ProcessExecutionTimeoutException()
    {
    }

    public ProcessExecutionTimeoutException(string? message) : base(message)
    {
    }

    public ProcessExecutionTimeoutException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ProcessExecutionTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
