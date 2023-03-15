using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace OcelotApiGw.Extensions;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services
            .AddCustomCors(configuration)
            .AddCustomOcelot(configuration)
            .AddCustomAuthentication(configuration);

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseCors("CorsPolicy");
        app.UseOcelot().Wait();

        return app;
    }

    private static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
    {
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


    private static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication()
            .AddJwtBearer("IdentityApiKey", x =>
            {
                x.Authority = configuration["IdentityUrl"];
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidAudiences = new[] { "usermanagement", "dictionary", "testing" }
                };
            });

        return services;
    }

    private static IServiceCollection AddCustomOcelot(this IServiceCollection services, IConfiguration configuration)
    {
        var builder = services.AddOcelot();

        builder.AddCacheManager(x =>
        {
            x.WithDictionaryHandle();
        });

        return services;
    }
}
