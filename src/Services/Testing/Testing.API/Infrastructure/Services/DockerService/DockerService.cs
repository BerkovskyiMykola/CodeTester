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
    Task<TerminalOutput> ExecuteImageAsync(string imageName, string containerName, int timeoutMiliseconds);
    Task<ContainerInfo> InspectAsync(string containerName);
    Task DeleteContainerAsync(string containerName);
    Task DeleteImageAsync(string imageName);
}

class DockerService : IDockerService
{
    private const int ExitCode = 0;
    private const int BuildTimeout = 60000;
    private const int CommandTimeout = 10000;

    private const string ContainerInfoFormat = "--format=\"{\\\"Status\\\": \\\"{{.State.Status}}\\\", \\\"CreationTime\\\": " +
            "\\\"{{.Created}}\\\", \\\"StartTime\\\": \\\"{{.State.StartedAt}}\\\", \\\"EndTime\\\": \\\"{{.State.FinishedAt}}\\\", \\\"ExitCode\\\": " +
            "{{.State.ExitCode}}, \\\"Error\\\": \\\"{{.State.Error}}\\\"}\"";

    private readonly ITerminalService _terminalService;
    private readonly ILogger<DockerService> _logger;

    public DockerService(ILogger<DockerService> logger, ITerminalService terminalExecutor)
    {
        _logger = logger;
        _terminalService = terminalExecutor;
    }

    public async Task BuildImageAsync(string contextPath, string imageName, string dockerfileName)
    {
        if (string.IsNullOrEmpty(contextPath))
        {
            throw new ArgumentException($"{nameof(contextPath)} must not be null or empty");;
        }

        if (string.IsNullOrEmpty(imageName))
        {
            throw new ArgumentException($"{nameof(imageName)} must not be null or empty");
        }

        if (string.IsNullOrEmpty(dockerfileName))
        {
            throw new ArgumentException($"{nameof(dockerfileName)} must not be null or empty");
        }

        string dockerfilePath = contextPath + "/" + dockerfileName;
        var command = new string[] { "docker", "image", "build", "-f", dockerfilePath, "-t", imageName, contextPath };

        await ExecuteDockerCommandAsync(command, BuildTimeout);
    }

    public async Task<TerminalOutput> ExecuteImageAsync(string imageName, string containerName, int timeoutMiliseconds)
    {
        if (string.IsNullOrEmpty(imageName))
        {
            throw new ArgumentException($"{nameof(imageName)} must not be null or empty");
        }

        if (string.IsNullOrEmpty(containerName))
        {
            throw new ArgumentException($"{nameof(containerName)} must not be null or empty");
        }

        if (timeoutMiliseconds <= 0)
        {
            throw new ArgumentException($"{nameof(timeoutMiliseconds)} should be a positive value");
        }

        var runCommand = new string[] { "docker", "run", "--name", containerName, imageName };
        var stopCommand = new string[] { "docker", "stop", containerName };

        try
        {
            var containerOutput = await _terminalService.ExecuteCommand(runCommand, timeoutMiliseconds);

            return containerOutput;
        }
        catch (TerminalExecutionException e)
        {
            throw new DockerOperationExecutionException(e.Message);
        }
        catch (TerminalExecutionTimeoutException)
        {
            throw new DockerOperationTimeoutException($"The Docker command exceeded the {timeoutMiliseconds} Miliseconds allowed for its execution");
        }
        finally
        {
            await _terminalService.ExecuteCommand(stopCommand, timeoutMiliseconds);
        }
    }

    public async Task<ContainerInfo> InspectAsync(string containerName)
    {
        if (string.IsNullOrEmpty(containerName))
        {
            throw new ArgumentException($"{nameof(containerName)} must not be null or empty");
        }

        var command = new string[] { "docker", "inspect", ContainerInfoFormat, containerName };
        var terminalOutput = await ExecuteDockerCommandAsync(command, CommandTimeout);

        return JsonSerializer.Deserialize<ContainerInfo>(terminalOutput.StandardOutput)!;
    }

    public async Task DeleteContainerAsync(string containerName)
    {
        if (string.IsNullOrEmpty(containerName))
        {
            throw new ArgumentException($"{nameof(containerName)} must not be null or empty");
        }

        var command = new string[] { "docker", "rm", "-f", containerName };
        await ExecuteDockerCommandAsync(command, CommandTimeout);
    }

    public async Task DeleteImageAsync(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
        {
            throw new ArgumentException($"{nameof(imageName)} must not be null or empty");
        }

        var command = new string[] { "docker", "rmi", "-f", imageName };
        await ExecuteDockerCommandAsync(command, CommandTimeout);
    }

    private async Task<TerminalOutput> ExecuteDockerCommandAsync(string[] command, int timeoutMiliseconds)
    {
        try
        {
            var terminalOutput = await _terminalService.ExecuteCommand(command, timeoutMiliseconds);

            _logger.LogDebug("Command logs: {0} {1}", terminalOutput.StandardOutput, terminalOutput.StandardError);

            if (terminalOutput.Status != ExitCode)
            {
                _logger.LogWarning("Command failed: {0}", string.Join(" ", command));
                throw new DockerOperationFailedException(terminalOutput.StandardError);
            }

            return terminalOutput;
        }
        catch (TerminalExecutionException e)
        {
            throw new DockerOperationExecutionException(e.Message);
        }
        catch (TerminalExecutionTimeoutException)
        {
            throw new DockerOperationTimeoutException($"The Docker command exceeded the {timeoutMiliseconds} Miliseconds allowed for its execution");
        }
    }
}