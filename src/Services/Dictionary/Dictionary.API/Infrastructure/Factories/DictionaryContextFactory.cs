using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Dictionary.API.Infrastructure.Factories;

public class DictionaryContextFactory : IDesignTimeDbContextFactory<DictionaryContext>
{
    public DictionaryContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<DictionaryContext>()
            .UseNpgsql(config["ConnectionString"], npgsqlOptionsAction: x => x.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));

        return new DictionaryContext(optionsBuilder.Options);
    }
}
