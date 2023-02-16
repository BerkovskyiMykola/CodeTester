using Testing.Core.Bases;
using Testing.Core.Domain.AggregatesModel.UserAggregate;

namespace Testing.Core.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    User Add(User user);
    User Update(User user);
    Task<User?> FindByIdAsync(Guid id);
}
