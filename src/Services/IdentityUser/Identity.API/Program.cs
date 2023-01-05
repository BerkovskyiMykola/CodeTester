using DataAccess.Data;
using DataAccess.Entities;
using Identity.API.Configuration;
using Identity.API.Data;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.API;

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

        var clientUrls = new Dictionary<string, string>();

        clientUrls.Add("Spa", builder.Configuration.GetValue<string>("SpaClient") ?? "");

        builder.Services.AddIdentityServer()
            //.AddConfigurationStore(options =>
            //{
            //    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
            //        sqlServerOptionsAction: sqlOptions =>
            //        {
            //            sqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
            //            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            //        });
            //})
            //.AddOperationalStore(options =>
            //{
            //    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
            //        sqlServerOptionsAction: sqlOptions =>
            //        {
            //            sqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
            //            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            //        });
            //})
            .AddInMemoryIdentityResources(Config.GetResources())
            .AddInMemoryApiResources(Config.GetApis())
            .AddInMemoryClients(Config.GetClients(clientUrls))
            .AddAspNetIdentity<ApplicationUser>()
            .AddDeveloperSigningCredential();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseIdentityServer();

        app.UseRouting();

        app.MigrateDbContext<ApplicationDbContext>((context, services) =>
        {
            var logger = services.GetRequiredService<ILogger<ApplicationDbContextSeed>>();
            var passwordHasher = services.GetRequiredService<IPasswordHasher<ApplicationUser>>();

            new ApplicationDbContextSeed()
                .SeedAsync(context, logger, passwordHasher)
                .Wait();
        });

        //app.MigrateDbContext<PersistedGrantDbContext>((_, __) => { });

        //app.MigrateDbContext<ConfigurationDbContext>((context, services) =>
        //{
        //    //new ConfigurationDbContextSeed()
        //    //    .SeedAsync(context, app.Configuration)
        //    //    .Wait();
        //});

        app.Run();
    }
}