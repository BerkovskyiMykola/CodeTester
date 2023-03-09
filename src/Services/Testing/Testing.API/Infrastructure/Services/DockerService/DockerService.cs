using System.Text.Json;
using Testing.API.Infrastructure.Models;
using Testing.API.Infrastructure.Services.DockerService.Exceptions;
using Testing.API.Infrastructure.Services.DockerService.Models;
using Testing.API.Infrastructure.Services.TerminalService;
using Testing.API.Infrastructure.Services.TerminalService.Exceptions;

namespace Testing.API.Infrastructure.Services.DockerService;

public interface IDockerService
{
    Task BuildImageAsync(string contextPath, string imageName, string dockerfileName);
    Task<TerminalOutput> RunContainerAsync(string imageName, string containerName, int timeout, float maxCpus, Dictionary<string, string> envVariables);
    Task<ContainerInfo> InspectAsync(string containerName);
    Task DeleteContainerAsync(string containerName);
    Task DeleteImageAsync(string imageName);
}

class DockerService : IDockerService
{
    private const int EXIT_CODE = 0;
    private const int BuildTimeout = 60000;
    private const int CommandTimeout = 10000;

    private const string ContainerInfoFormat = "--format=\"{\\\"Status\\\": \\\"{{.State.Status}}\\\", \\\"CreationTime\\\": " +
            "\\\"{{.Created}}\\\", \\\"StartTime\\\": \\\"{{.State.StartedAt}}\\\", \\\"EndTime\\\": \\\"{{.State.FinishedAt}}\\\", \\\"ExitCode\\\": " +
            "{{.State.ExitCode}}, \\\"Error\\\": \\\"{{.State.Error}}\\\"}\"";

    private readonly ITerminalService _terminalService;

    public DockerService(ITerminalService terminalExecutor)
    {
        _terminalService = terminalExecutor;
    }

    public async Task BuildImageAsync(string contextPath, string imageName, string dockerfileName)
    {
        string dockerfilePath = contextPath + "/" + dockerfileName;
        var command = new string[] { "docker", "image", "build", "-f", dockerfilePath, "-t", imageName, contextPath };
        var terminalOutput = await ExecuteDockerCommandAsync(command, BuildTimeout);

        if (terminalOutput.Status != EXIT_CODE)
        {
            throw new DockerOperationFailedException(terminalOutput.StandardError);
        }
    }

    public async Task<TerminalOutput> RunContainerAsync(string imageName, string containerName, int timeout, float maxCpus, Dictionary<string, string> envVariables)
    {
        var command = BuildDockerRunCommand(containerName, envVariables, maxCpus, imageName);

        return await ExecuteDockerCommandAsync(command, timeout);
    }

    private string[] BuildDockerRunCommand(string containerName, Dictionary<string, string> envVariables, float cpus, string imageName)
    {
        //docker run --name [containerName] (-e [envKey=envValue])* --cpus=[cpu] [imageName]
        var command = new List<string>() { "docker", "run", "--name", containerName };

        foreach (var i in envVariables)
        {
            command.Add("-e");
            command.Add(i.Key + "=" + i.Value);
        }

        var cpuParam = "--cpus=" + cpus;
        command.Add(cpuParam);
        command.Add(imageName);

        return command.ToArray();
    }

    public async Task<ContainerInfo> InspectAsync(string containerName)
    {
        var command = new string[] { "docker", "inspect", ContainerInfoFormat, containerName };
        var terminalOutput = await ExecuteDockerCommandAsync(command, CommandTimeout);

        if (terminalOutput.Status != EXIT_CODE)
        {
            throw new DockerOperationFailedException(terminalOutput.StandardError);
        }

        return JsonSerializer.Deserialize<ContainerInfo>(terminalOutput.StandardOutput)!;
    }

    public async Task DeleteContainerAsync(string containerName)
    {
        var command = new string[] { "docker", "rm", "-f", containerName };
        var terminalOutput = await ExecuteDockerCommandAsync(command, CommandTimeout);

        if (terminalOutput.Status != EXIT_CODE)
        {
            throw new DockerOperationFailedException(terminalOutput.StandardError);
        }
    }

    public async Task DeleteImageAsync(string imageName)
    {
        var command = new string[] { "docker", "rmi", "-f", imageName };
        var terminalOutput = await ExecuteDockerCommandAsync(command, CommandTimeout);

        if (terminalOutput.Status != EXIT_CODE)
        {
            throw new DockerOperationFailedException(terminalOutput.StandardError);
        }
    }

    private async Task<TerminalOutput> ExecuteDockerCommandAsync(string[] command, int timeout)
    {
        try
        {
            return await _terminalService.ExecuteCommand(command, timeout);
        }
        catch (TerminalExecutionException e)
        {
            throw new DockerOperationFailedException(e.Message);
        }
        catch (TerminalExecutionTimeoutException e)
        {
            throw new DockerOperationTimeoutException(e.Message);
        }
    }
}