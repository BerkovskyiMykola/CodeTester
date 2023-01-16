using Dictionary.API;
using Dictionary.API.Filters;
using Dictionary.API.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dictionary HTTP API",
        Version = "v1",
        Description = "The Dictionary Service HTTP API"
    });
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows()
        {
            Password = new OpenApiOAuthFlow()
            {
                TokenUrl = new Uri($"{builder.Configuration["IdentityUrlExternal"]}/connect/token"),
                Scopes = new Dictionary<string, string>()
                {
                    { "dictionary", "Dictionary API" }
                }
            }
        }
    });

    options.OperationFilter<AuthorizeCheckOperationFilter>();
});

builder.Services.AddDbContext<DictionaryDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration["ConnectionString"], sqlOptions =>
    {
        sqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
    });

});

// prevent from mapping "sub" claim to nameidentifier.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityUrl"];
        options.RequireHttpsMetadata = false;
        options.Audience = "dictionary";
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
        .SetIsOriginAllowed((host) => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty;
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dictionary.API V1");
        options.OAuthClientId("spa.client");
        options.OAuthAppName("Dictionary Swagger UI");
    });
}

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MigrateDbContext<DictionaryDBContext>((context, services) =>
{
    var logger = services.GetRequiredService<ILogger<DictionaryDBContext>>();

    new DictionaryDBContextSeed()
        .SeedAsync(context, logger)
        .Wait();
});

app.Run();
