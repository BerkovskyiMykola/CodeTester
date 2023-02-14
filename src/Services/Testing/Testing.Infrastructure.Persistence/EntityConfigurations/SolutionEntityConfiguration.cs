﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Testing.Core.Domain.AggregatesModel.SolutionAggregate;
using DomainTask = Testing.Core.Domain.AggregatesModel.TaskAggregate.Task;

namespace Testing.Infrastructure.Persistence.EntityConfigurations;


public class SolutionEntityConfiguration
    : IEntityTypeConfiguration<Solution>
{
    public void Configure(EntityTypeBuilder<Solution> solutionConfiguration)
    {
        solutionConfiguration.HasKey(x => x.Id);

        solutionConfiguration.Ignore(x => x.DomainEvents);

        solutionConfiguration
            .OwnsOne(x => x.User);

        solutionConfiguration
            .OwnsOne(x => x.Value);

        solutionConfiguration
            .Property<Guid>("_taskId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("TaskId")
            .IsRequired(true);

        solutionConfiguration.HasOne<DomainTask>()
            .WithMany()
            .IsRequired(true)
            .HasForeignKey("_taskId");
    }
}