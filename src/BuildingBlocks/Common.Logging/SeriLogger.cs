using Microsoft.Extensions.Configuration;
using Serilog;

namespace Common.Logging;

public static class SeriLogger
{
    public static ILogger CreateSerilogLogger(IConfiguration configuration, string appName)
    {
        return new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.WithProperty("ApplicationContext", appName)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Seq(configuration["SeqServerUrl"]!)
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }
}
