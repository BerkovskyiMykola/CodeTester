﻿namespace Testing.API.Infrastructure.Services.TestExecutionSerivce.Executions.Models;

public class ExecutionOptions
{
    public string TemplatePath { get; set; } = string.Empty;
    public string TemplateDockerfileName { get; set; } = string.Empty;
    public string TemplateExecutionFileName { get; set; } = string.Empty;
    public string ExecutionsFolderPath { get; set; } = string.Empty;
    public int TimeLimit { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Tests { get; set; } = string.Empty;
}
