﻿using Dictionary.API.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.API.Persistence.Configurations;

public class ProgrammingLanguageConfigurations : IEntityTypeConfiguration<ProgrammingLanguage>
{
    public void Configure(EntityTypeBuilder<ProgrammingLanguage> builder)
    {
        builder.Property(b => b.Id).UseIdentityAlwaysColumn();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(256);
    }
}