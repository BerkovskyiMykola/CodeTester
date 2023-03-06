using Dictionary.API.Protos;
using EventBus.Messages.Common;
using Grpc.Interceptors;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using StudentProfile.API.Infrastructure.EventBusConsumers;
using System.IdentityModel.Tokens.Jwt;
using Testing.API.Application.Queries.Solutions;
using Testing.API.Application.Queries.Tasks;
using Testing.API.Infrastructure;
using Testing.API.Infrastructure.EventBusConsumers;
using Testing.API.Infrastructure.Filters;
using Testing.API.Infrastructure.Options;
using Testing.API.Infrastructure.Services;
using Testing.API.Infrastructure.Services.DictionaryService;
using Testing.API.Infrastructure.Services.DockerService;
using Testing.API.Infrastructure.Services.TerminalService;
using Testing.Core.Domain.Repositories;
using Testing.Infrastructure.Persistence;
using Testing.Infrastructure.Persistence.Repositories;

namespace Testing.API.Extensions;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services
            .AddCustomSwagger(configuration)
            .AddCustomAuthentication(configuration)
            .AddCustomMvc(configuration)
            .AddCustomHealthCheck(configuration)
            .AddCustomConfiguration(configuration)
            .AddCustomDbContext(configuration)
            .AddEventBus(configuration)
            .AddGrpcServices(configuration)
            .AddCustomIntegrations(configuration);

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Testing.API V1");
            options.EnablePersistAuthorization();
            options.OAuthClientId("testing-swagger");
            options.OAuthScopes("openid", "testing", "roles");
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

        return app;
    }

    public static WebApplication ApplyMigrations(this WebApplication app)
    {
        app.MigrateDbContext<TestingContext>((context, services) =>
        {
            var logger = services.GetRequiredService<ILogger<TestingContext>>();

            new TestingContextSeed()
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
                Title = "Testing HTTP API",
                Version = "v1",
                Description = "The Testing Service HTTP API"
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

            options.MapType<TimeSpan>(() => new OpenApiSchema
            {
                Type = "string",
                Example = new OpenApiString("00:00:01")
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
                options.Audience = "testing";
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

        hcBuilder.AddRabbitMQ(
            configuration["EventBusHostAddress"]!,
            name: "usermanagement-rabbitmqbus-check",
            tags: new string[] { "rabbitmqbus" });

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

        services.Configure<ExecutionSettings>(configuration.GetSection(nameof(ExecutionSettings)));

        return services;
    }

    private static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TestingContext>(options =>
        {
            options.UseNpgsql(configuration["ConnectionString"], sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
            });
        });

        return services;
    }

    private static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(config =>
        {
            config.AddConsumer<UserProfileUpdatedConsumer>();
            config.AddConsumer<UserProfileCreatedConsumer>();

            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["EventBusHostAddress"]);

                cfg.ReceiveEndpoint(EventBusConstants.TestingQueue, c =>
                {
                    c.ConfigureConsumer<UserProfileUpdatedConsumer>(context);
                    c.ConfigureConsumer<UserProfileCreatedConsumer>(context);
                });
            });
        });

        return services;
    }

    public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ClientLoggerInterceptor>();

        services.AddGrpcClient<DictionaryGrpc.DictionaryGrpcClient>(options =>
        {
            options.Address = new Uri(configuration["DictionaryGrpcUrl"]!);
        }).AddInterceptor<ClientLoggerInterceptor>();

        services.AddScoped<IDictionaryService, DictionaryService>();

        return services;
    }

    private static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IIdentityService, IdentityService>();

        services.AddScoped<ISolutionRepository, SolutionRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<ISolutionQueries, SolutionQueries>();
        services.AddScoped<ITaskQueries, TaskQueries>();

        services.AddScoped<ITerminalService, TerminalService>();
        services.AddScoped<IDockerService, DockerService>();

        return services;
    }
}
