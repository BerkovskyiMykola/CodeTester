using System.Diagnostics;
using Testing.API.Infrastructure.Models;
using Testing.API.Infrastructure.Services.TerminalService.Exceptions;

namespace Testing.API.Infrastructure.Services.TerminalService;

public interface ITerminalService
{
    Task<TerminalOutput> ExecuteCommand(string[] command, int timeoutMiliseconds);
}

public class TerminalService : ITerminalService
{
    private readonly ILogger<TerminalService> _logger;

    public TerminalService(ILogger<TerminalService> logger)
    {
        _logger = logger;
    }

    public async Task<TerminalOutput> ExecuteCommand(string[] command, int timeoutMiliseconds)
    {
        if (timeoutMiliseconds <= 0)
        {
            throw new ArgumentException("Timeout should be a positive value");
        }

        if (command.Length == 0)
        {
            throw new ArgumentException("Command should have at least one element");
        }

        try
        {
            _logger.LogInformation("Start executing the command");
            using var cmd = new Process();
            cmd.StartInfo.FileName = "/bin/bash";
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            foreach (var line in command)
            {
                cmd.StandardInput.Write(line);
                cmd.StandardInput.Write(" ");
            }
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();

            var stopWatch = new Stopwatch();

            stopWatch.Start();
            if (!cmd.WaitForExit(timeoutMiliseconds))
            {
                try
                {
                    cmd.Kill();

                    _logger.LogInformation("The process exceeded the {0} Miliseconds allowed for its execution", timeoutMiliseconds);
                    throw new TerminalExecutionTimeoutException($"The process exceeded the {timeoutMiliseconds} Miliseconds allowed for its execution");
                }
                catch (InvalidOperationException)
                {
                    cmd.WaitForExit();

                    return new TerminalOutput(
                        await cmd.StandardOutput.ReadToEndAsync(),
                        await cmd.StandardError.ReadToEndAsync(),
                        (int)stopWatch.Elapsed.TotalSeconds,
                        cmd.ExitCode);
                }
            }
            stopWatch.Stop();

            return new TerminalOutput(
                await cmd.StandardOutput.ReadToEndAsync(),
                await cmd.StandardError.ReadToEndAsync(),
                (int)stopWatch.Elapsed.TotalSeconds,
                cmd.ExitCode);
        }
        catch (Exception e)
        {
            _logger.LogError("Unexpected error: {0}", e);
            throw new TerminalExecutionException("Fatal error for command " + string.Join(" ", command) + " : " + e.Message);
        }
    }
}