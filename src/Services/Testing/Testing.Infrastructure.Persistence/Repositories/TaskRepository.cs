using Microsoft.EntityFrameworkCore;
using Testing.Core.Bases;
using Testing.Core.Domain.Repositories;
using DomainTask = Testing.Core.Domain.AggregatesModel.TaskAggregate.Task;

namespace Testing.Infrastructure.Persistence.Repositories;

public class TaskRepository
    : ITaskRepository
{
    private readonly TestingContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public TaskRepository(TestingContext context)
    {
        _context = context;
    }

    public DomainTask Add(DomainTask task)
    {
        if (task.IsTransient())
        {
            return _context.Tasks.Add(task).Entity;
        }

        return task;
    }

    public DomainTask Update(DomainTask task)
    {
        return _context.Tasks.Update(task).Entity;
    }

    public DomainTask Delete(DomainTask task)
    {
       return _context.Tasks.Remove(task).Entity;
    }

    public async Task<DomainTask?> FindByIdAsync(Guid id)
    {
        return await _context.Tasks
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
