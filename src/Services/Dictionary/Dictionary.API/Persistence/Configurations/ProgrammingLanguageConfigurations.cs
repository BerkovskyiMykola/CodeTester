using Dictionary.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dictionary.API.Persistence.Configurations;

public class ProgrammingLanguageConfigurations : IEntityTypeConfiguration<ProgrammingLanguage>
{
    public void Configure(EntityTypeBuilder<ProgrammingLanguage> builder)
    {
        builder.Property(b => b.Id).UseSerialColumn();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(256);
    }
}
