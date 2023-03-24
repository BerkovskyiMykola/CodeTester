using Microsoft.EntityFrameworkCore;
using Testing.Core.Domain.AggregatesModel.UserAggregate;
using Testing.Core.Domain.Repositories;
using Common.Models.Domain;

namespace Testing.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TestingContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public UserRepository(TestingContext context)
    {
        _context = context;
    }

    public User Add(User user)
    {
        return _context.Users.Add(user).Entity;
    }

    public User Update(User user)
    {
        return _context.Users.Update(user).Entity;
    }

    public async Task<User?> FindByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
