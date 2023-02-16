using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.API.Factories;

public class ConfigurationDbContextFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
{
    public ConfigurationDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
           .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
           .AddJsonFile("appsettings.json")
           .AddEnvironmentVariables()
           .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>()
            .UseSqlServer(config["ConnectionString"], sqlServerOptionsAction: x => x.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));

        var storeOptions = new ConfigurationStoreOptions();

        return new ConfigurationDbContext(optionsBuilder.Options) { StoreOptions = storeOptions };
    }
}