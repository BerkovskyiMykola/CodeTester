using Microsoft.EntityFrameworkCore;
using Testing.Core.Bases;
using Testing.Core.Domain.AggregatesModel.UserAggregate;
using Testing.Core.Domain.Repositories;

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
        if (user.IsTransient())
        {
            return _context.Users.Add(user).Entity;
        }

        return user;
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
