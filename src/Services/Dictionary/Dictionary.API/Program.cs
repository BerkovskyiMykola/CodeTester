using Common.Logging;
using Dictionary.API.Extensions;
using Serilog;

var configuration = GetConfiguration();

Log.Logger = SeriLogger.CreateSerilogLogger(configuration, AppName);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", AppName);

    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.CaptureStartupErrors(false);
    builder.WebHost.UseConfiguration(configuration);
    builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    Log.Information("Applying migrations ({ApplicationContext})...", AppName);

    app.ApplyMigrations();

    Log.Information("Starting web host ({ApplicationContext})...", AppName);

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}


IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    return builder.Build();
}

public partial class Program
{

    public static readonly string Namespace = typeof(HostingExtensions).Namespace!;
    public static readonly string AppName = Namespace;
}