using Common.Models.Domain;
using DomainTask = Testing.Core.Domain.AggregatesModel.TaskAggregate.Task;

namespace Testing.Core.Domain.Repositories;

public interface ITaskRepository : IRepository<DomainTask>
{
    DomainTask Add(DomainTask task);
    DomainTask Update(DomainTask task);
    DomainTask Delete(DomainTask task);
    Task<DomainTask?> FindByIdAsync(Guid id);
}
