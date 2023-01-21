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
            .AddCustomIntegrations()
            .AddScoped<IEmailSender, EmailSender>();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.EnablePersistAuthorization();
            options.OAuthClientId("usermanagement-swagger");
            options.OAuthScopes("openid", "usermanagement", "roles");
            options.OAuthUsePkce();
        });

        app.UseRouting();
        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}