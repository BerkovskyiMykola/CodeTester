using System.Text.Json;
using Testing.API.Infrastructure.Services.DockerService.Exceptions;
using Testing.API.Infrastructure.Services.DockerService.Models;
using Testing.API.Infrastructure.Services.TerminalService;
using Testing.API.Infrastructure.Services.TerminalService.Exceptions;
using Testing.API.Infrastructure.Services.TerminalService.Models;

namespace Testing.API.Infrastructure.Services.DockerService;

public interface IDockerService
{
    Task<string> BuildImageAsync(string contextPath, string imageName, string dockerfileName);
    Task<TerminalOutput> RunContainerAsync(string imageName, string containerName, int timeout, float maxCpus, Dictionary<string, string> envVariables);
    Task<ContainerInfo> InspectAsync(string containerName);
    Task DeleteContainerAsync(string containerName);
    Task DeleteImageAsync(string imageName);
}

class DockerService : IDockerService
{
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

    public async Task<string> BuildImageAsync(string contextPath, string imageName, string dockerfileName)
    {
        string dockerfilePath = contextPath + "/" + dockerfileName;
        var buildCommand = new string[] { "docker", "image", "build", "-f", dockerfilePath, "-t", imageName, contextPath };
        return await ExecuteDockerCommandAsync(buildCommand, BuildTimeout);
    }

    public async Task<TerminalOutput> RunContainerAsync(string imageName, string containerName, int timeout, float maxCpus, Dictionary<string, string> envVariables)
    {
        var command = BuildDockerCommand(containerName, envVariables, maxCpus, imageName);

        return await _terminalService.ExecuteCommand(command, timeout);
    }

    private string[] BuildDockerCommand(string containerName, Dictionary<string, string> envVariables, float cpus, string imageName)
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
        string containerInfo = await ExecuteDockerCommandAsync(command, CommandTimeout);

        return JsonSerializer.Deserialize<ContainerInfo>(containerInfo)!;
    }

    public async Task DeleteContainerAsync(string containerName)
    {
        var command = new string[] { "docker", "rm", "-f", containerName };
        await ExecuteDockerCommandAsync(command, CommandTimeout);
    }

    public async Task DeleteImageAsync(string imageName)
    {
        var command = new string[] { "docker", "rmi", "-f", imageName };
        await ExecuteDockerCommandAsync(command, CommandTimeout);
    }

    private async Task<string> ExecuteDockerCommandAsync(string[] command, int timeout)
    {
        try
        {
            var terminalOutput = await _terminalService.ExecuteCommand(command, timeout);
            if (!string.IsNullOrEmpty(terminalOutput.StandardError))
            {
                throw new DockerOperationFailedException(terminalOutput.StandardError);
            }
            return terminalOutput.StandardOutput;
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