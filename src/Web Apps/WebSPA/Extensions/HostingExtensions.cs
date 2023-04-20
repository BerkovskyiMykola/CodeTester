using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebSPA.Extensions;

public static class HostingExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services
            .AddCustomHealthCheck(configuration)
            .AddCustomMvc(configuration)
            .AddCustomSpaStaticFiles(configuration);

        return builder;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        var configuration = app.Configuration;

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        var antiForgery = app.Services.GetRequiredService<IAntiforgery>();
        app.Use(next => context =>
        {
            string path = context.Request.Path.Value!;

            if (string.Equals(path, "/", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(path, "/index.html", StringComparison.OrdinalIgnoreCase))
            {
                // The request token has to be sent as a JavaScript-readable cookie, 
                // and Angular uses it by default.
                var tokens = antiForgery.GetAndStoreTokens(context);
                context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
                    new CookieOptions() { HttpOnly = false });
            }

            return next(context);
        });

        app.UseDefaultFiles();
        app.UseStaticFiles();

        // This will make the application to respond with the index.html and the rest of the assets present on the configured folder (at AddSpaStaticFiles() (wwwroot))
        if (!app.Environment.IsDevelopment())
        {
            app.UseSpaStaticFiles();
        }

        app.UseRouting();

        app.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "ClientApp";

            if (app.Environment.IsDevelopment())
            {
                // use the SpaServices extension method for angular, that will make the application to run "ng serve" for us, when in development.
                spa.UseAngularCliServer(npmScript: "start");
            }
        });

        return app;
    }

    private static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        hcBuilder.AddUrlGroup(new Uri(configuration["IdentityUrlHC"]!), name: "identityapi-check", tags: new string[] { "identityapi" });

        return services;
    }

    private static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

        services.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        return services;
    }

    private static IServiceCollection AddCustomSpaStaticFiles(this IServiceCollection services, IConfiguration configuration)
    {
        // In production, the Angular files will be served from this directory
        services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = "ClientApp/dist/aspnetcorespa";
        });

        return services;
    }
}
