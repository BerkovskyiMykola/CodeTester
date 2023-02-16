using Dictionary.API.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dictionary.API.Infrastructure.EntityConfigurations;

public class DifficultyEntityConfigurations : IEntityTypeConfiguration<Difficulty>
{
    public void Configure(EntityTypeBuilder<Difficulty> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(ci => ci.Id)
            .UseHiLo("difficulty_hilo")
            .IsRequired(true);

        builder.Property(x => x.Name)
            .IsRequired(true)
            .HasMaxLength(50);
    }
}
