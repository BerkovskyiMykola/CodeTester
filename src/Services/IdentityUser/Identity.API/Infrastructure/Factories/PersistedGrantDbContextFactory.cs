using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.API.Factories
{
    public class PersistedGrantDbContextFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
    {
        public PersistedGrantDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PersistedGrantDbContext>()
                .UseSqlServer(config["ConnectionString"], sqlServerOptionsAction: x => x.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));

            var storeOptions = new OperationalStoreOptions();

            return new PersistedGrantDbContext(optionsBuilder.Options) { StoreOptions = storeOptions };
        }
    }
}