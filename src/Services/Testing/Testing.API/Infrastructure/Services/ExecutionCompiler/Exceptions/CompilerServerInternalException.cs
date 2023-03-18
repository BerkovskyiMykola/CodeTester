using System.Runtime.Serialization;

namespace Testing.API.Infrastructure.Services.ExecutionCompiler.Exceptions;

public class CompilerServerInternalException : Exception
{
    public CompilerServerInternalException()
    {
    }

    public CompilerServerInternalException(string? message) : base(message)
    {
    }

    public CompilerServerInternalException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CompilerServerInternalException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
