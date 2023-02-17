using Microsoft.EntityFrameworkCore;
using Testing.Core.Bases;
using Testing.Core.Domain.AggregatesModel.SolutionAggregate;
using Testing.Core.Domain.Repositories;

namespace Testing.Infrastructure.Persistence.Repositories;

public class SolutionRepository : ISolutionRepository
{
    private readonly TestingContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public SolutionRepository(TestingContext context)
    {
        _context = context;
    }

    public async Task<Solution?> FindByIdAsync(Guid id)
    {
        return await _context.Solutions
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Solution Upsert(Solution solution)
    {
        _context.Entry(solution).State = solution.IsTransient() ?
            EntityState.Added :
            EntityState.Modified;

        return solution;
    }
}
