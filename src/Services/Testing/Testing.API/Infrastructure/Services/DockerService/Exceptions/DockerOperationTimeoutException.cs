using System.Runtime.Serialization;

namespace Testing.API.Infrastructure.Services.DockerService.Exceptions;

public class DockerOperationTimeoutException : Exception
{
    public DockerOperationTimeoutException()
    {
    }

    public DockerOperationTimeoutException(string? message) : base(message)
    {
    }

    public DockerOperationTimeoutException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected DockerOperationTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
