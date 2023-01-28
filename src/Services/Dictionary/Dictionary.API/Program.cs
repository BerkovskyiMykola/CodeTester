using Dictionary.API;
using Dictionary.API.Persistence;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var configuration = GetConfiguration();
Log.Logger = CreateSerilogLogger(configuration);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", AppName);

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services
        .AddCustomSwagger(configuration)
        .AddCustomDbContext(configuration)
        .AddCustomAuthentication(configuration)
        .AddCustomMvc(configuration)
        .AddCustomHealthCheck(configuration);

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnablePersistAuthorization();
        options.OAuthClientId("dictionary-swagger");
        options.OAuthScopes("openid", "dictionary", "roles");
        options.OAuthUsePkce();
    });

    app.UseCors("CorsPolicy");

    app.UseRouting();
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

    Log.Information("Applying migrations ({ApplicationContext})...", AppName);

    app.MigrateDbContext<DictionaryDBContext>((context, services) =>
    {
        var logger = services.GetRequiredService<ILogger<DictionaryDBContext>>();

        new DictionaryDBContextSeed()
            .SeedAsync(context, logger)
            .Wait();
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