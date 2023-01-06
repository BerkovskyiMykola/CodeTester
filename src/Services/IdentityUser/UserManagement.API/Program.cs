using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration["ConnectionString"];

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            });
        });

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        //app.MigrateDbContext<ApplicationDbContext>((context, services) =>
        //{
        //    var logger = services.GetRequiredService<ILogger<ApplicationDbContextSeed>>();
        //    var passwordHasher = services.GetRequiredService<IPasswordHasher<ApplicationUser>>();

        //    new ApplicationDbContextSeed()
        //        .SeedAsync(context, logger, passwordHasher)
        //        .Wait();
        //});

        app.Run();
    }
}