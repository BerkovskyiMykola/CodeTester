using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Testing.Core.Domain.AggregatesModel.UserAggregate;

namespace Testing.Infrastructure.Persistence.EntityConfigurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.DomainEvents);

        builder.OwnsOne(x => x.Email);

        builder.OwnsOne(x => x.Profile);
    }
}
