using Serilog;
using WebStatus;

var configuration = GetConfiguration();

Log.Logger = CreateSerilogLogger(configuration);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", AppName);

    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.CaptureStartupErrors(false);
    builder.WebHost.UseConfiguration(configuration);
    builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());

    builder.Host.UseSerilog();

    builder.Services
        .AddCustomHealthCheck(configuration)
        .AddCustomHealthChecksUI(configuration)
        .AddOptions()
        .AddMvc();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
    }

    app.UseHealthChecksUI(config =>
    {
        config.ResourcesPath = "/ui/resources";
        config.UIPath = "/hc-ui";
    });

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapDefaultControllerRoute();

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

Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", AppName)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(configuration["SeqServerUrl"]!)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
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

    public static readonly string Namespace = typeof(ConfigureServices).Namespace!;
    public static readonly string AppName = Namespace;
}