using Common.Logging;
using Duende.Bff.Yarp;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using System.Security.Claims;
using WebSPA;

var configuration = GetConfiguration();

Log.Logger = SeriLogger.CreateSerilogLogger(configuration, AppName);

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
        .AddCustomAuthorization(configuration)
        .AddCustomAuthentication(configuration)
        .AddBff()
        .AddRemoteApis();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

    app.UseAuthentication();

    app.UseBff();

    app.UseAuthorization();

    app.MapBffManagementEndpoints();

    app.MapGet("/local/identity", LocalIdentityHandler).AsBffApiEndpoint();

    app.MapRemoteBffApiEndpoint("/remote", configuration["Ocelotapigw"]!).RequireAccessToken(Duende.Bff.TokenType.User);

    app.MapHealthChecks("/hc", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    app.MapHealthChecks("/liveness", new HealthCheckOptions
    {
        Predicate = r => r.Name.Contains("self")
    });

    // DEV ONLY
    // e.g. Replace internal to external identity adress
    app.Use(async (httpcontext, next) =>
    {
        await next();
        if (httpcontext.Response.StatusCode == StatusCodes.Status302Found)
        {
            var containerHost = builder.Configuration["IdentityUrl"];
            var authority = builder.Configuration["IdentityUrlExternal"];

            if (!containerHost!.Equals(authority, StringComparison.OrdinalIgnoreCase))
            {
                string location = httpcontext.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Location]!;
                httpcontext.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Location] =
                        location.Replace(containerHost, authority);
            }

        }
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

IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    return builder.Build();
}

[Authorize]
static IResult LocalIdentityHandler(ClaimsPrincipal user, HttpContext context)
{
    var name = user.FindFirst("name")?.Value ?? user.FindFirst("sub")?.Value;
    return Results.Json(new { message = "Local API Success!", user = name });
}

public partial class Program
{

    public static readonly string Namespace = typeof(ConfigureServices).Namespace!;
    public static readonly string AppName = Namespace;
}