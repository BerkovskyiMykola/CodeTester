using Dictionary.API.Controllers;
using Dictionary.API.GrpcServices;
using Dictionary.API.Infrastructure;
using Dictionary.API.Infrastructure.Filters;
using Grpc.Interceptors;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Dictionary.API.Extensions;

public static class HostingExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services
            .AddCustomGrpc(configuration)
            .AddCustomSwagger(configuration)
            .AddCustomDbContext(configuration)
            .AddCustomAuthentication(configuration)
            .AddCustomMvc(configuration)
            .AddCustomHealthCheck(configuration)
            .AddCustomConfiguration(configuration);

        return builder;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dictionary.API V1");
            options.EnablePersistAuthorization();
            options.OAuthClientId("dictionary-swagger");
            options.OAuthScopes("openid", "dictionary", "roles");
            options.OAuthUsePkce();
        });

        app.UseRouting();
        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapGrpcService<DictionaryService>();
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
        app.MigrateDbContext<DictionaryContext>((context, services) =>
        {
            var logger = services.GetRequiredService<ILogger<DictionaryContext>>();

            new DictionaryContextSeed()
                .SeedAsync(context, logger)
                .Wait();
        });

        return app;
    }


    private static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Dictionary HTTP API",
                Version = "v1",
                Description = "The Dictionary Service HTTP API"
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

    private static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DictionaryContext>(options =>
        {
            options.UseNpgsql(configuration["ConnectionString"], sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(DictionaryContext).Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
            });

        });

        return services;
    }

    private static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["IdentityUrl"];
                options.RequireHttpsMetadata = false;
                options.Audience = "dictionary";
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
        .AddApplicationPart(typeof(DifficultiesController).Assembly)
        .AddApplicationPart(typeof(ProgrammingLanguagesController).Assembly)
        .AddApplicationPart(typeof(TaskTypesController).Assembly)
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

    private static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        hcBuilder.AddNpgSql(
            configuration["ConnectionString"]!,
            name: "DictionaryDB-check",
            tags: new string[] { "DictionaryDB" });

        return services;
    }

    private static IServiceCollection AddCustomGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ServerLoggerInterceptor>();

        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
            options.Interceptors.Add<ServerLoggerInterceptor>();
        });

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
