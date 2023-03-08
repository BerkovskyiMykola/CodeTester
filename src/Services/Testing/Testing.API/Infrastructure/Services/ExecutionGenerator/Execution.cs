using Testing.API.Infrastructure.Services.ExecutionGenerator.Models;

namespace Testing.API.Infrastructure.Services.ExecutionGenerator;

public class Execution
{
    public Guid Id { get; init; }
    private readonly ExecutionOptions _options;

    private readonly string _executionPath;

    public Execution(ExecutionOptions options)
    {
        Id = Guid.NewGuid();
        _options = options;

        _executionPath = Path.Combine(options.ExecutionsFolderPath, Id.ToString());
    }

    public void CreateExecutionDirectory()
    {
        Directory.CreateDirectory(_executionPath);

        CopyTemplateFilesToExecutionDirectory();
        PrepareDockerFileToExecution();
        PrepareExecutionFileToExecution();
    }

    private void CopyTemplateFilesToExecutionDirectory()
    {
        var source = new DirectoryInfo(_options.TemplatePath);
        var target = new DirectoryInfo(_executionPath);

        foreach (FileInfo fi in source.GetFiles())
        {
            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
        }
    }

    private void PrepareDockerFileToExecution()
    {
        var dockerFile = Path.Combine(_executionPath, _options.TemplateDockerfileName);
        var content = File.ReadAllText(dockerFile).Replace("{timeLimit}", _options.TimeLimit.ToString());
        File.WriteAllText(dockerFile, content);
    }

    private void PrepareExecutionFileToExecution()
    {
        var executionFile = Path.Combine(_executionPath, _options.TemplateDockerfileName);
        File.WriteAllText(executionFile, _options.Code);
    }

    public void DeleteExecutionDirectory()
    {
        Directory.Delete(_executionPath, true);
    }
}
