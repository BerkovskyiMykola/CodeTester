using Dictionary.API.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dictionary.API.Infrastructure.EntityConfigurations;

public class TaskTypeEntityConfigurations : IEntityTypeConfiguration<TaskType>
{
    public void Configure(EntityTypeBuilder<TaskType> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(ci => ci.Id)
            .UseHiLo("TaskType_hilo")
            .IsRequired(true);

        builder.Property(x => x.Name)
            .IsRequired(true)
            .HasMaxLength(50);
    }
}
