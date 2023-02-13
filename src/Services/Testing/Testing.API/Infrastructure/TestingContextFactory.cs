using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Testing.Infrastructure.Persistence;

namespace Testing.API.Infrastructure
{
    public class TestingContextFactory : IDesignTimeDbContextFactory<TestingContext>
    {
        public TestingContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TestingContext>();

            optionsBuilder.UseNpgsql(config["ConnectionString"], npgsqlOptionsAction: o => o.MigrationsAssembly("Testing.API"));

            return new TestingContext(optionsBuilder.Options);
        }
    }
}
