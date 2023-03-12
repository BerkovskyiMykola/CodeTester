using Testing.API.Infrastructure.Services.DockerService;
using Testing.API.Infrastructure.Services.ExecutionCompiler.Models;
using Testing.API.Infrastructure.Services.ExecutionGenerator;

namespace Testing.API.Infrastructure.Services.ExecutionCompiler;

public interface IExecutionCompiler
{
    Task<ExecutionResult> Execute(Execution execution);
}

public class ExecutionCompiler : IExecutionCompiler
{
    private const string IMAGE_PREFIX_NAME = "image-";

    private readonly IDockerService _dockerService;

    public ExecutionCompiler(IDockerService dockerService)
    {
        _dockerService = dockerService;
    }

    public async Task<ExecutionResult> Execute(Execution execution)
    {
        execution.CreateExecutionDirectory();
        await _dockerService.BuildImageAsync(execution.ExecutionPath, IMAGE_PREFIX_NAME + execution.Id, execution.DockerfileName);

        var result = await _dockerService.RunContainerAsync(IMAGE_PREFIX_NAME + execution.Id, execution.Id.ToString(), 20000, 8, new());

        execution.DeleteExecutionDirectory();
        await _dockerService.DeleteContainerAsync(execution.Id.ToString());
        await _dockerService.DeleteImageAsync(IMAGE_PREFIX_NAME + execution.Id);

        return new ExecutionResult();
    }
}