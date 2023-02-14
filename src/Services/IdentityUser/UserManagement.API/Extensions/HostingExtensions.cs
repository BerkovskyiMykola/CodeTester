using DataAccess.Data;
using DataAccess.Entities;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using UserManagement.API.Controllers;
using UserManagement.API.Grpc;
using UserManagement.API.Infrastructure.Filters;
using UserManagement.API.Infrastructure.Services;
using UserManagement.API.Infrastructure.Services.EmailService;

namespace UserManagement.API.Extensions;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services
            .AddCustomGrpc(configuration)
            .AddCustomMvc(configuration)
            .AddCustomDbContext(configuration)
            .AddCustomDbContext(configuration)
            .AddCustomIdentity(configuration)
            .AddCustomSwagger(configuration)
            .AddCustomAuthentication(configuration)
            .AddCustomConfiguration(configuration)
            .AddCustomIntegrations(configuration)
            .AddCustomHealthCheck(configuration)
            .AddEventBus(configuration);

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dictionary.API V1");
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
        app.MapGrpcService<UserManagementService>();
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


    private static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "UserManagement HTTP API",
                Version = "v1",
                Description = "The UserManagement Service HTTP API"
            });

            var scheme = new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{configuration["IdentityUrlExternal"]}/connect/authorize"),
                        TokenUrl = new Uri($"{configuration["IdentityUrlExternal"]}/connect/token")
                    }
                },
                Type = SecuritySchemeType.OAuth2
            };

            options.AddSecurityDefinition("OAuth", scheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Id = "OAuth", Type = ReferenceType.SecurityScheme }
                    },
                    new List<string> { }
                }
            });
        });

        return services;
    }

    private static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // prevent from mapping "sub" claim to nameidentifier.
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = configuration["IdentityUrl"];
            options.RequireHttpsMetadata = false;
            options.Audience = "usermanagement";
        });

        return services;
    }

    private static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<EmailConfiguration>(configuration.GetSection(nameof(EmailConfiguration)));
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

    private static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration["ConnectionString"], sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            });
        });

        return services;
    }

    private static IServiceCollection AddCustomIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromHours(3);
        });

        return services;
    }

    private static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
    {
        // Add framework services.
        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(HttpGlobalExceptionFilter));
        })
        // Added for functional tests
        .AddApplicationPart(typeof(AccountController).Assembly)
        .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });

        return services;
    }

    private static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddScoped<IEmailSender, EmailSender>();

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

        hcBuilder.AddRabbitMQ(
            configuration["EventBusHostAddress"]!,
            name: "usermanagement-rabbitmqbus-check",
            tags: new string[] { "rabbitmqbus" });

        return services;
    }

    private static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["EventBusHostAddress"]);
            });
        });

        return services;
    }

    private static IServiceCollection AddCustomGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
        });

        return services;
    }
}
