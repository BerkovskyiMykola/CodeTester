namespace Testing.API.Infrastructure.Services.DockerService.Models;

public record ContainerInfo
{
    public DateTime CreationTime { get; }
    public string Status { get; }
    public string Error { get; }
    public int ExitCode { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }

    public ContainerInfo(DateTime creationTime, string status, string error, int exitCode, DateTime startTime, DateTime endTime)
    {
        CreationTime = creationTime;
        Status = status;
        Error = error;
        ExitCode = exitCode;
        StartTime = startTime;
        EndTime = endTime;
    }
}
