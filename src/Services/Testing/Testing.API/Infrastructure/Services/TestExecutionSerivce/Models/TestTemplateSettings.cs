namespace Testing.API.Infrastructure.Services.TestExecutionSerivce.Models;

public class TestTemplateSettings
{
    public TestTemplate CSharp { get; set; } = new TestTemplate();
    public TestTemplate Python { get; set; } = new TestTemplate();
    public TestTemplate JavaScript { get; set; } = new TestTemplate();
}
