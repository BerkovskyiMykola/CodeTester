namespace Testing.API.Infrastructure.Options;

public class ExecutionSettings
{
    public string ExecutionsRelativePath { get; set; } = string.Empty;
    public Dictionary<string, TestTemplate> Templates { get; set; } = new Dictionary<string, TestTemplate>();
}
