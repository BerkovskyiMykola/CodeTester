using DataAccess.Data;
using DataAccess.Entities;
using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using HealthChecks.UI.Client;
using Identity.API.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Identity.API.Extensions;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services
            .AddCustomDbContext(configuration)
            .AddCustomIdentity(configuration)
            .AddCustomIdentityServer(configuration)
            .AddCustomAuthentication(configuration)
            .AddCustomConfiguration(configuration)
            .AddCustomHealthCheck(configuration)
            .AddRazorPages();

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

        return app;
    }

    public static WebApplication ApplyMigrations(this WebApplication app)
    {
        app.MigrateDbContext<ApplicationContext>((context, services) =>
        {
            var logger = services.GetRequiredService<ILogger<ApplicationContextSeed>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            new ApplicationContextSeed()
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

        return app;
    }


    private static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseSqlServer(configuration["ConnectionString"], sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            });
        });

        return services;
    }

    private static IServiceCollection AddCustomIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection AddCustomIdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;

                options.IssuerUri = "null";
                options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
            })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            })
            .AddAspNetIdentity<ApplicationUser>();

        return services;
    }

    private static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                options.ClientId = configuration["Authentication:Google:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
            });

        return services;
    }

    private static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        hcBuilder.AddSqlServer(
            configuration["ConnectionString"]!,
            name: "IdentityDB-check",
            tags: new string[] { "IdentityDB" });

        return services;
    }

    private static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to the errors property for additional details."
                };

                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json", "application/problem+xml" }
                };
            };
        });

        return services;
    }
}
