using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;

namespace OcelotApiGw;

public static class ConfigureServices
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddAuthentication()
        //    .AddJwtBearer("IdentityApiKey", x =>
        //    {
        //        x.Authority = configuration["IdentityUrl"];
        //        x.RequireHttpsMetadata = false;
        //        x.TokenValidationParameters = new TokenValidationParameters()
        //        {
        //            ValidAudiences = new[] { "usermanagment", "dictionary", "testing", "testingagg" }
        //        };
        //    });

        return services;
    }

    public static IServiceCollection AddCustomOcelot(this IServiceCollection services, IConfiguration configuration)
    {
        var builder = services.AddOcelot();

        builder.AddCacheManager(x =>
         {
             x.WithDictionaryHandle();
         });

        return services;
    }
}
