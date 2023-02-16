using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.API.Factories;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
           .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
           .AddJsonFile("appsettings.json")
           .AddEnvironmentVariables()
           .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlServer(config["ConnectionString"], sqlServerOptionsAction: x => x.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));

        return new ApplicationContext(optionsBuilder.Options);
    }
}