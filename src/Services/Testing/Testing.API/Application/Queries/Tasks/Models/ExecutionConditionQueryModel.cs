namespace Testing.API.Application.Queries.Tasks.Models;

public class ExecutionConditionQueryModel
{
    public string Tests { get; set; } = string.Empty;
    public TimeSpan TimeLimit { get; set; }
}
