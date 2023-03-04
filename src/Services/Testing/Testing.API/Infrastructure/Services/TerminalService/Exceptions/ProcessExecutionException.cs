using System.Runtime.Serialization;

namespace Testing.API.Infrastructure.Services.TerminalService.Exceptions;

public class ProcessExecutionException : Exception
{
    public ProcessExecutionException()
    {
    }

    public ProcessExecutionException(string? message) : base(message)
    {
    }

    public ProcessExecutionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ProcessExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
