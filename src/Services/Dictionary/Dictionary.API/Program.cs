using Common.Logging;
using Dictionary.API.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using System.Net;

var configuration = GetConfiguration();

Log.Logger = SeriLogger.CreateSerilogLogger(configuration, AppName);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", AppName);

    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.CaptureStartupErrors(false);
    builder.WebHost.UseConfiguration(configuration);
    builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
    builder.WebHost.ConfigureKestrel(options =>
    {
        var ports = GetDefinedPorts(configuration);

        options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        });

        options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        });
    });

    builder.Host.UseSerilog();

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

(int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
{
    var grpcPort = config.GetValue("GRPC_PORT", 81);
    var port = config.GetValue("PORT", 80);
    return (port, grpcPort);
}

public partial class Program
{

    public static readonly string Namespace = typeof(HostingExtensions).Namespace!;
    public static readonly string AppName = Namespace;
}