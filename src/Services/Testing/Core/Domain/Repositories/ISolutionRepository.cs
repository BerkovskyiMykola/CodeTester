using Testing.Core.Bases;
using Testing.Core.Domain.AggregatesModel.SolutionAggregate;

namespace Testing.Core.Domain.Repositories;

public interface ISolutionRepository : IRepository<Solution>
{
    Solution Upsert(Solution solution);
    Task<Solution> FindByIdAsync(Guid id);
}
