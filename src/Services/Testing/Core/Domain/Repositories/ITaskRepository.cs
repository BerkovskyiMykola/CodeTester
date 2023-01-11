using Core.Bases;
using DomainTask = Core.Domain.AggregatesModel.TaskAggregate.Task;

namespace Core.Domain.Repositories;

public interface ITaskRepository : IRepository<DomainTask>
{
    Task Add(DomainTask task);
    Task Update(DomainTask task);
    Task<DomainTask> FindByIdAsync(Guid id);
}
