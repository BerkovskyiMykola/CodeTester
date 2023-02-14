using Dictionary.API.Infrastructure.Entities;
using Dictionary.API.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.API.Infrastructure;

public class DictionaryContext : DbContext
{
    public DbSet<Difficulty> Difficulties => Set<Difficulty>();
    public DbSet<ProgrammingLanguage> ProgrammingLanguages => Set<ProgrammingLanguage>();
    public DbSet<TaskType> TaskTypes => Set<TaskType>();

    public DictionaryContext(DbContextOptions<DictionaryContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DifficultyEntityConfigurations());
        modelBuilder.ApplyConfiguration(new ProgrammingLanguageEntityConfigurations());
        modelBuilder.ApplyConfiguration(new TaskTypeEntityConfigurations());
    }
}
