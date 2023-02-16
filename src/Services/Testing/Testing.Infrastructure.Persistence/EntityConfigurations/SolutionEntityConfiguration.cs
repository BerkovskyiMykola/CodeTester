using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Testing.Core.Domain.AggregatesModel.SolutionAggregate;
using Testing.Core.Domain.AggregatesModel.UserAggregate;
using DomainTask = Testing.Core.Domain.AggregatesModel.TaskAggregate.Task;

namespace Testing.Infrastructure.Persistence.EntityConfigurations;


public class SolutionEntityConfiguration : IEntityTypeConfiguration<Solution>
{
    public void Configure(EntityTypeBuilder<Solution> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.DomainEvents);

        builder.OwnsOne(x => x.Value);

        builder
            .Property<Guid>("_taskId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("TaskId")
            .IsRequired(true);

        builder
            .Property<Guid>("_userId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("UserId")
            .IsRequired(true);

        builder.HasOne<DomainTask>()
            .WithMany()
            .IsRequired(true)
            .HasForeignKey("_taskId");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey("_userId");
    }
}
