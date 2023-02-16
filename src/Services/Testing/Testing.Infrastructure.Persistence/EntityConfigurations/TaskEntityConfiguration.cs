using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainTask = Testing.Core.Domain.AggregatesModel.TaskAggregate.Task;

namespace Testing.Infrastructure.Persistence.EntityConfigurations;

public class TaskEntityConfiguration : IEntityTypeConfiguration<DomainTask>
{
    public void Configure(EntityTypeBuilder<DomainTask> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.DomainEvents);

        builder.OwnsOne(x => x.Title);

        builder.OwnsOne(x => x.Description);

        builder.OwnsOne(x => x.Difficulty);

        builder.OwnsOne(x => x.Type);

        builder.OwnsOne(x => x.ProgrammingLanguage);

        builder.OwnsOne(x => x.SolutionExample);

        builder.OwnsOne(x => x.ExecutionCondition);
    }
}
