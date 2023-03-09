namespace Testing.API.Infrastructure.Services.ExecutionGenerator;

public class Execution
{    
    private const string EXECUTION_FOLDER_PREFIX_NAME = "execution-";

    private readonly Guid _id;
    private readonly int _timeLimit;
    private readonly string _code;
    private readonly string _dockerfileName;
    private readonly string _executionFileName;
    private readonly string _templatePath;
    private readonly string _executionPath;

    public Guid Id => _id;
    public string ExecutionPath => _executionPath;
    public string DockerfileName => _dockerfileName;

    public Execution(
        string templatePath,
        string templateDockerfileName,
        string templateExecutionFileName,
        string executionsFolderPath,
        int timeLimit,
        string code)
    {
        _id = Guid.NewGuid();
        _timeLimit = timeLimit;
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
        PrepareDockerFileToExecution();
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

    private void PrepareDockerFileToExecution()
    {
        var dockerFile = Path.Combine(_executionPath, _dockerfileName);
        var content = File.ReadAllText(dockerFile).Replace("{timeLimit}", _timeLimit.ToString());
        File.WriteAllText(dockerFile, content);
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
