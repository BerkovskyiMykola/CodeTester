using Common.Logging;
using DataAccess.Data;
using DataAccess.Entities;
using Duende.IdentityServer.EntityFramework.DbContexts;
using HealthChecks.UI.Client;
using Identity.API;
using Identity.API.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
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

    builder.Host.UseSerilog();

    builder.Services
        .AddCustomDbContext(configuration)
        .AddCustomIdentity(configuration)
        .AddCustomIdentityServer(configuration)
        .AddCustomAuthentication(configuration)
        .AddCustomConfiguration(configuration)
        .AddCustomHealthCheck(configuration)
        .AddRazorPages();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
    }

    app.UseStaticFiles();
    app.UseRouting();

    // Fix a problem with chrome. Chrome enabled a new feature "Cookies without SameSite must be secure", 
    // the cookies should be expired from https, but in Microservices, the internal communication in aks and docker compose is http.
    // To avoid this problem, the policy of cookies should be in Lax mode.
    app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

    app.UseIdentityServer();

    app.UseAuthorization();
    app.MapRazorPages().RequireAuthorization();

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

    app.MigrateDbContext<ApplicationDbContext>((context, services) =>
    {
        var logger = services.GetRequiredService<ILogger<ApplicationDbContextSeed>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        new ApplicationDbContextSeed()
            .SeedAsync(context, logger, userManager, roleManager)
            .Wait();
    });

    app.MigrateDbContext<PersistedGrantDbContext>((_, __) => { });

    app.MigrateDbContext<ConfigurationDbContext>((context, services) =>
    {
        new ConfigurationDbContextSeed()
            .SeedAsync(context, app.Configuration)
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