using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace OcelotApiGw;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile($"ocelot.json", true, true);

        builder.Services.AddOcelot().AddCacheManager(x =>
        {
            x.WithDictionaryHandle();
        });

        var app = builder.Build();

        app.UseOcelot().Wait();

        app.Run();
    }
}