using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebSPA;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        builder.Services.AddAuthorization();

        builder.Services
            .AddBff()
            .AddRemoteApis();

        builder.Services
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
                options.Authority = builder.Configuration["IdentityUrl"];
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

        var app = builder.Build();

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

        app.MapRemoteBffApiEndpoint("/remote", builder.Configuration["Ocelotapigw"]!).RequireAccessToken(Duende.Bff.TokenType.User);

        // DEV ONLY
        // e.g. Replace internal to external identity adress
        app.Use(async (httpcontext, next) =>
        {
            await next();
            if (httpcontext.Response.StatusCode == StatusCodes.Status302Found)
            {
                var containerHost = builder.Configuration["IdentityUrl"];
                var authority = builder.Configuration["IdentityUrlExternal"];

                if (!containerHost.Equals(authority, StringComparison.OrdinalIgnoreCase))
                {
                    string location = httpcontext.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Location];
                    httpcontext.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Location] =
                            location.Replace(containerHost, authority);
                }

            }
        });

        app.Run();
    }

    [Authorize]
    public static IResult LocalIdentityHandler(ClaimsPrincipal user, HttpContext context)
    {
        var name = user.FindFirst("name")?.Value ?? user.FindFirst("sub")?.Value;
        return Results.Json(new { message = "Local API Success!", user = name });
    }
}
