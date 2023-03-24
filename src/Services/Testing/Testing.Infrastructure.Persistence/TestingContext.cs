using Common.Models.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Testing.Core.Domain.AggregatesModel.SolutionAggregate;
using Testing.Core.Domain.AggregatesModel.UserAggregate;
using Testing.Infrastructure.Persistence.EntityConfigurations;
using DomainTask = Testing.Core.Domain.AggregatesModel.TaskAggregate.Task;

namespace Testing.Infrastructure.Persistence;

public class TestingContext : DbContext, IUnitOfWork
{
    public DbSet<DomainTask> Tasks => Set<DomainTask>();
    public DbSet<Solution> Solutions => Set<Solution>();
    public DbSet<User> Users => Set<User>();

    private readonly IMediator? _mediator;

    public TestingContext(DbContextOptions<TestingContext> options) : base(options) { }

    public TestingContext(DbContextOptions<TestingContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SolutionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TaskEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        if (_mediator != null)
        {
            await _mediator.DispatchDomainEventsAsync(this);
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        return true;
    }
}
