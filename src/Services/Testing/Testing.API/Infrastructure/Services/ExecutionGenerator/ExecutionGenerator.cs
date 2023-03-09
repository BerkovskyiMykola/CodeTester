using Microsoft.Extensions.Options;
using Testing.API.Infrastructure.Options;

namespace Testing.API.Infrastructure.Services.ExecutionGenerator;

public interface IExecutionGenerator
{
    Execution CreateExecution(string code, int timeLimit, string language);
}

public class ExecutionGenerator : IExecutionGenerator
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ExecutionSettings _executionSettings;

    public ExecutionGenerator(
        IWebHostEnvironment webHostEnvironment,
        IOptions<ExecutionSettings> executionSettingsOption)
    {
        _webHostEnvironment = webHostEnvironment;
        _executionSettings = executionSettingsOption.Value;
    }

    public Execution CreateExecution(string code, int timeLimit, string language)
    {
        var template = _executionSettings.Templates.GetValueOrDefault(language);

        if (template == null)
        {
            throw new Exception($"Language {language} is not registered");
        }

        return new Execution(
            Path.Combine(_webHostEnvironment.WebRootPath, template.RelativePath),
            template.DockerfileName,
            template.ExecutionFileName,
            Path.Combine(_webHostEnvironment.WebRootPath, _executionSettings.ExecutionsRelativePath),
            timeLimit,
            code
            );
    }
}