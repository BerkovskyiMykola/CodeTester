namespace Testing.API.Infrastructure.Models;

public record TerminalOutput
{
    public string StandardOutput { get; }
    public string StandardError { get; }
    public int ExecutionDuration { get; }
    public int Status { get; }

    public TerminalOutput(string standardOutput, string standardError, int executionDuration, int status)
    {
        StandardOutput = standardOutput;
        StandardError = standardError;
        ExecutionDuration = executionDuration;
        Status = status;
    }
}
