﻿using Dictionary.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dictionary.API.Persistence.Configurations;

public class TaskTypeConfigurations : IEntityTypeConfiguration<TaskType>
{
    public void Configure(EntityTypeBuilder<TaskType> builder)
    {
        builder.Property(b => b.Id).UseSerialColumn();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(256);
    }
}
