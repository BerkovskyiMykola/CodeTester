using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using UserManagement.API;
using UserManagement.API.EmailService;

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
        .AddCustomMvc(configuration)
        .AddCustomDbContext(configuration)
        .AddCustomDbContext(configuration)
        .AddCustomIdentity(configuration)
        .AddCustomSwagger(configuration)
        .AddCustomAuthentication(configuration)
        .AddCustomConfiguration(configuration)
        .AddCustomIntegrations(configuration)
        .AddCustomHealthCheck(configuration)
        .AddScoped<IEmailSender, EmailSender>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnablePersistAuthorization();
        options.OAuthClientId("usermanagement-swagger");
        options.OAuthScopes("openid", "usermanagement", "roles");
        options.OAuthUsePkce();
    });

    app.UseRouting();
    app.UseCors("CorsPolicy");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapHealthChecks("/hc", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    app.MapHealthChecks("/liveness", new HealthCheckOptions
    {
        Predicate = r => r.Name.Contains("self")
    });

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