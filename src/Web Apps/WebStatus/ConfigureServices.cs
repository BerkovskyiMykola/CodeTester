using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebStatus;

public static class ConfigureServices
{
    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        return services;
    }

    public static IServiceCollection AddCustomHealthChecksUI(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecksUI();

        hcBuilder.AddInMemoryStorage();

        return services;
    }
}
