using Common.Models.Base;
using Testing.API.Infrastructure.Services.DockerService;
using Testing.API.Infrastructure.Services.DockerService.Exceptions;
using Testing.API.Infrastructure.Services.ExecutionCompiler.Exceptions;
using Testing.API.Infrastructure.Services.ExecutionGenerator;

namespace Testing.API.Infrastructure.Services.ExecutionCompiler;

public interface IExecutionCompiler
{
    Task<Result<string>> Execute(Execution execution);
}

public class ExecutionCompiler : IExecutionCompiler
{
    private const string IMAGE_PREFIX_NAME = "image-";

    private readonly IDockerService _dockerService;
    private readonly ILogger<ExecutionCompiler> _logger;

    public ExecutionCompiler(ILogger<ExecutionCompiler> logger, IDockerService dockerService)
    {
        _logger = logger;
        _dockerService = dockerService;
    }

    public async Task<Result<string>> Execute(Execution execution)
    {
        BuildExecutionEnvironment(execution);
        try
        {
            await BuildImageAsync(execution);

            var result = await _dockerService.ExecuteImageAsync(IMAGE_PREFIX_NAME + execution.Id, execution.Id.ToString(), execution.TimeoutMiliseconds);

            if (!string.IsNullOrWhiteSpace(result.StandardError))
            {
                return Result.Fail<string>(result.StandardError);
            }

            return Result.Ok<string>("Success");
        }
        catch (DockerOperationTimeoutException)
        {
            return Result.Fail<string>("Timeout");
        }
        catch (CompilerServerInternalException)
        {
            return Result.Fail<string>("Build failed");
        }
        catch (Exception e)
        {
            _logger.LogError("Error while executing: {}", e);
            throw new CompilerServerInternalException(e.Message);
        }
        finally
        {
            await DeleteContainerAsync(execution);
            await DeleteImageAsync(execution);
            DeleteExecutionEnvironment(execution);
        }
    }

    private void BuildExecutionEnvironment(Execution execution)
    {
        try
        {
            execution.CreateExecutionDirectory();
        }
        catch (Exception e)
        {
            _logger.LogError("Error while building execution environment: {}", e);
            throw new CompilerServerInternalException(e.Message);
        }
    }

    private async Task BuildImageAsync(Execution execution)
    {
        try
        {
            await _dockerService.BuildImageAsync(execution.ExecutionPath, IMAGE_PREFIX_NAME + execution.Id, execution.DockerfileName);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while building image: {}", e);
            throw new CompilerServerInternalException("Build failed");
        }
    }

    private async Task DeleteContainerAsync(Execution execution)
    {
        try
        {
            await _dockerService.DeleteContainerAsync(execution.Id.ToString());
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting container: {0}", e);
        }
    }

    private async Task DeleteImageAsync(Execution execution)
    {
        try
        {
            await _dockerService.DeleteImageAsync(IMAGE_PREFIX_NAME + execution.Id);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting image: {0}", e);
        }
    }

    private void DeleteExecutionEnvironment(Execution execution)
    {
        try
        {
            execution.DeleteExecutionDirectory();
        }
        catch (Exception e)
        {
            _logger.LogError("Error while trying to delete execution directory, {0}", e);
        }
    }
}