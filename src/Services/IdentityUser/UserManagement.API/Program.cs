using UserManagement.API.EmailService;

namespace UserManagement.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;

        builder.Services
            .AddCustomMvc()
            .AddCustomDbContext(configuration)
            .AddCustomDbContext(configuration)
            .AddCustomIdentity()
            .AddCustomSwagger(configuration)
            .AddCustomAuthentication(configuration)
            .AddCustomConfiguration(configuration)
            .AddScoped<IEmailSender, EmailSender>();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = string.Empty;
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "UserManagement.API V1");
            options.OAuthClientId("spa.client");
            options.OAuthAppName("UserManagement Swagger UI");
        });

        app.UseRouting();
        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}