using System.Runtime.Serialization;

namespace Testing.API.Infrastructure.Services.DockerService.Exceptions;

public class DockerOperationExecutionException : Exception
{
    public DockerOperationExecutionException()
    {
    }

    public DockerOperationExecutionException(string? message) : base(message)
    {
    }

    public DockerOperationExecutionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected DockerOperationExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
