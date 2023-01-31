using Testing.Core.Bases;
using DomainTask = Testing.Core.Domain.AggregatesModel.TaskAggregate.Task;

namespace Testing.Core.Domain.Repositories;

public interface ITaskRepository : IRepository<DomainTask>
{
    DomainTask Add(DomainTask task);
    DomainTask Update(DomainTask task);
    Task Delete(Guid id);
    Task<DomainTask> FindByIdAsync(Guid id);
}
