using System;
using System.Diagnostics;
using Testing.API.Infrastructure.Services.TerminalService.Exceptions;
using Testing.API.Infrastructure.Services.TerminalService.Models;

namespace Testing.API.Infrastructure.Services.TerminalService;

public interface ITerminalService
{
    Task<TerminalOutput> ExecuteCommand(string[] command, int timeout);
}

public class TerminalService : ITerminalService
{
    public async Task<TerminalOutput> ExecuteCommand(string[] command, int timeout)
    {
        if (timeout <= 0)
        {
            throw new ArgumentException("Timeout should be a positive value");
        }

        if (command.Length == 0)
        {
            throw new ArgumentException("Command should have at least one element");
        }

        try
        {
            var cmd = new Process();
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
            if (!cmd.WaitForExit(timeout))
            {
                try
                {
                    cmd.Kill();
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

                throw new ProcessExecutionTimeoutException("The process takes longer to execute than the time limit set");
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
            throw new CommandExecutionException("Fatal error for command " + string.Join(" ", command) + " : " + e.Message);
        }
    }
}