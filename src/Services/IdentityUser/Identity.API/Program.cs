using DataAccess.Data;
using DataAccess.Entities;
using Duende.IdentityServer.EntityFramework.DbContexts;
using HealthChecks.UI.Client;
using Identity.API;
using Identity.API.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .Enrich.WithProperty("ApplicationContext", AppName)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(configuration["SeqServerUrl"]!)
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", AppName);

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

public partial class Program
{

    public static string Namespace = typeof(IHostExtensions).Namespace!;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}