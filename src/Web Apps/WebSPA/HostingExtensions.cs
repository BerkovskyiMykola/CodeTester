using Duende.Bff.Yarp;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebSPA;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services
            .AddCustomHealthCheck(configuration)
            .AddCustomAuthorization(configuration)
            .AddCustomAuthentication(configuration)
            .AddBff()
            .AddRemoteApis();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        var configuration = app.Configuration;

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

        app.UseAuthentication();

        app.UseBff();

        app.UseAuthorization();

        app.MapBffManagementEndpoints();

        app.MapGet("/local/identity", LocalIdentityHandler).AsBffApiEndpoint();

        app.MapRemoteBffApiEndpoint("/remote", configuration["Ocelotapigw"]!).RequireAccessToken(Duende.Bff.TokenType.User);

        app.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });

        // DEV ONLY
        // e.g. Replace internal to external identity adress
        app.Use(async (httpcontext, next) =>
        {
            await next();
            if (httpcontext.Response.StatusCode == StatusCodes.Status302Found)
            {
                var containerHost = configuration["IdentityUrl"]!;
                var authority = configuration["IdentityUrlExternal"]!;

                string location = httpcontext.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Location]!;
                httpcontext.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Location] =
                        location.Replace(containerHost, authority);
            }
        });

        return app;
    }

    [Authorize]
    private static IResult LocalIdentityHandler(ClaimsPrincipal user, HttpContext context)
    {
        var name = user.FindFirst("name")?.Value ?? user.FindFirst("sub")?.Value;
        return Results.Json(new { message = "Local API Success!", user = name });
    }

    private static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        hcBuilder.AddUrlGroup(new Uri(configuration["IdentityUrlHC"]!), name: "identityapi-check", tags: new string[] { "identityapi" });

        return services;
    }

    private static IServiceCollection AddCustomAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
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

                options.ClaimActions.MapJsonKey("role", "role");

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
            });

        return services;
    }
}
