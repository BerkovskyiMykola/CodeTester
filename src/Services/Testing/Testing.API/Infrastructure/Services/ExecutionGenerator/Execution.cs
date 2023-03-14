namespace Testing.API.Infrastructure.Services.ExecutionGenerator;

public class Execution
{
    private const string EXECUTION_FOLDER_PREFIX_NAME = "execution-";

    private readonly Guid _id;
    private readonly string _code;
    private readonly int _timeoutMiliseconds;
    private readonly string _dockerfileName;
    private readonly string _executionFileName;
    private readonly string _templatePath;
    private readonly string _executionPath;

    public Guid Id => _id;
    public string ExecutionPath => _executionPath;
    public string DockerfileName => _dockerfileName;
    public int TimeoutMiliseconds => _timeoutMiliseconds;

    public Execution(
        string templatePath,
        string templateDockerfileName,
        string templateExecutionFileName,
        string executionsFolderPath,
        int timeoutMiliseconds,
        string code)
    {
        _id = Guid.NewGuid();

        if (timeoutMiliseconds <= 0)
        {
            throw new ArgumentException($"{nameof(timeoutMiliseconds)} should be a positive value");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException($"{nameof(code)} must not be null or empty");
        }

        if (string.IsNullOrWhiteSpace(templateExecutionFileName))
        {
            throw new ArgumentException($"{nameof(templateExecutionFileName)} must not be null or empty");
        }

        if (string.IsNullOrWhiteSpace(templateDockerfileName))
        {
            throw new ArgumentException($"{nameof(templateDockerfileName)} must not be null or empty");
        }

        if (string.IsNullOrWhiteSpace(templatePath))
        {
            throw new ArgumentException($"{nameof(templatePath)} must not be null or empty");
        }

        _timeoutMiliseconds = timeoutMiliseconds;
        _code = code;

        _executionFileName = templateExecutionFileName;
        _dockerfileName = templateDockerfileName;

        _templatePath = templatePath;
        _executionPath = Path.Combine(executionsFolderPath, EXECUTION_FOLDER_PREFIX_NAME + _id);
    }

    public void CreateExecutionDirectory()
    {
        Directory.CreateDirectory(_executionPath);

        CopyTemplateFilesToExecutionDirectory();
        PrepareExecutionFileToExecution();
    }

    private void CopyTemplateFilesToExecutionDirectory()
    {
        var source = new DirectoryInfo(_templatePath);
        var target = new DirectoryInfo(_executionPath);

        foreach (FileInfo fi in source.GetFiles())
        {
            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
        }
    }

    private void PrepareExecutionFileToExecution()
    {
        var executionFile = Path.Combine(_executionPath, _executionFileName);
        File.WriteAllText(executionFile, _code);
    }

    public void DeleteExecutionDirectory()
    {
        Directory.Delete(_executionPath, true);
    }
}
