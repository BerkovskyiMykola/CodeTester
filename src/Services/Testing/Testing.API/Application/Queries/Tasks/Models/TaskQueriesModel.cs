namespace Testing.API.Application.Queries.Tasks.Models;

public class TaskQueriesModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public TaskQueriesModelDescription Description { get; set; } = new();
    public TaskQueriesModelDifficulty Difficulty { get; set; } = new();
    public TaskQueriesModelTaskType TaskType { get; set; } = new();
    public TaskQueriesModelProgrammingLanguage ProgrammingLanguage { get; set; } = new();
    public TaskQueriesModelSolutionExample SolutionExample { get; set; } = new();
    public TaskQueriesModelExecutionCondition ExecutionCondition { get; set; } = new();

    public DateTime CreateDate { get; set; }
}

public class TaskQueriesModelDescription
{
    public string Text { get; set; } = string.Empty;
    public string Examples { get; set; } = string.Empty;
    public string? SomeCases { get; set; }
    public string? Note { get; set; }
}

public class TaskQueriesModelDifficulty
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TaskQueriesModelTaskType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TaskQueriesModelProgrammingLanguage
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TaskQueriesModelSolutionExample
{
    public string? Description { get; set; }
    public string Solution { get; set; } = string.Empty;
}

public class TaskQueriesModelExecutionCondition
{
    public string Tests { get; set; } = string.Empty;
    public TimeSpan TimeLimit { get; set; }
}
