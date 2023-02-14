using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebStatus.Extensions;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services
            .AddCustomHealthCheck(configuration)
            .AddCustomHealthChecksUI(configuration)
            .AddOptions()
            .AddMvc();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
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

        app.MapDefaultControllerRoute();

        app.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });

        return app;
    }

    private static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        return services;
    }

    private static IServiceCollection AddCustomHealthChecksUI(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecksUI();

        hcBuilder.AddInMemoryStorage();

        return services;
    }
}
