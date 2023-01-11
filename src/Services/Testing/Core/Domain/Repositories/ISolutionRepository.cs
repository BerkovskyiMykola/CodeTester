using Core.Bases;
using Core.Domain.AggregatesModel.SolutionAggregate;

namespace Core.Domain.Repositories;

public interface ISolutionRepository : IRepository<Solution>
{
    Solution Add(Solution solution);
    Solution Upsert(Solution solution);
    Task<Solution> FindByIdAsync(Guid id);
}
