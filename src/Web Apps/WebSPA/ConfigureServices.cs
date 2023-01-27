using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.IdentityModel.Tokens.Jwt;

namespace WebSPA;

public static class ConfigureServices
{
    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        hcBuilder.AddUrlGroup(new Uri(configuration["IdentityUrlHC"]!), name: "identityapi-check", tags: new string[] { "identityapi" });

        return services;
    }

    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignOutScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";
                options.Authority = configuration["IdentityUrl"];
                options.RequireHttpsMetadata = false;
                options.ClientId = "bff";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                options.Scope.Add("roles");
                options.Scope.Add("usermanagement");
                options.Scope.Add("dictionary");

                //maybe useless
                options.ClaimActions.MapJsonKey("role", "role");

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
            });

        return services;
    }
}
