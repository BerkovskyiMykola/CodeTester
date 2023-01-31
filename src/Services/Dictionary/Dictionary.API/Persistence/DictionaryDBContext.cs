using Dictionary.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.API.Persistence;

public class DictionaryDBContext : DbContext
{
    public DbSet<Difficulty> Difficulties => Set<Difficulty>();
    public DbSet<ProgrammingLanguage> ProgrammingLanguages => Set<ProgrammingLanguage>();
    public DbSet<TaskType> TaskTypes => Set<TaskType>();

    public DictionaryDBContext(DbContextOptions<DictionaryDBContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
