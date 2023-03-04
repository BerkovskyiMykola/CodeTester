using System.Runtime.Serialization;

namespace Testing.API.Infrastructure.Services.DockerService.Exceptions;

public class DockerOperationFailedException : Exception
{
    public DockerOperationFailedException()
    {
    }

    public DockerOperationFailedException(string? message) : base(message)
    {
    }

    public DockerOperationFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected DockerOperationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
