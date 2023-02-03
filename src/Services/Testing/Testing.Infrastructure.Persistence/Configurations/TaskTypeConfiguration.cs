using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainTask = Testing.Core.Domain.AggregatesModel.TaskAggregate.Task;

namespace Testing.Infrastructure.Persistence.Configurations;

public class TaskTypeConfiguration
    : IEntityTypeConfiguration<DomainTask>
{
    public void Configure(EntityTypeBuilder<DomainTask> taskConfiguration)
    {
        taskConfiguration.HasKey(x => x.Id);

        taskConfiguration.Ignore(x => x.DomainEvents);

        taskConfiguration
            .OwnsOne(x => x.Title);

        taskConfiguration
            .OwnsOne(x => x.Description);

        taskConfiguration
            .OwnsOne(x => x.Difficulty);

        taskConfiguration
            .OwnsOne(x => x.Type);

        taskConfiguration
            .OwnsOne(x => x.ProgrammingLanguage);

        taskConfiguration
            .OwnsOne(x => x.SolutionExample);

        taskConfiguration
            .OwnsOne(x => x.ExecutionCondition);
    }
}
