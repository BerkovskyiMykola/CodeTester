namespace Testing.API.Infrastructure.Services.ExecutionCompiler.Models;

public class ExecutionResult
{
	public string Messsage { get; }

	public ExecutionResult(string message)
	{
        Messsage = message;
    }
}
