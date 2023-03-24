using Common.Models.Domain;
using Testing.Core.Domain.AggregatesModel.SolutionAggregate;

namespace Testing.Core.Domain.Repositories;

public interface ISolutionRepository : IRepository<Solution>
{
    Task<Solution?> FindByIdAsync(Guid id);
    Solution Add(Solution solution);
    Solution Update(Solution solution);
}
