using Dictionary.API.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dictionary.API.Infrastructure.EntityConfigurations;

public class ProgrammingLanguageEntityConfigurations : IEntityTypeConfiguration<ProgrammingLanguage>
{
    public void Configure(EntityTypeBuilder<ProgrammingLanguage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(ci => ci.Id)
            .UseHiLo("ProgrammingLanguage_hilo")
            .IsRequired(true);

        builder.Property(x => x.Name)
            .IsRequired(true)
            .HasMaxLength(50);
    }
}
