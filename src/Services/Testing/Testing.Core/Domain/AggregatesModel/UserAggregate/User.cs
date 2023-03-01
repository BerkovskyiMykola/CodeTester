using Testing.Core.Bases;

namespace Testing.Core.Domain.AggregatesModel.UserAggregate;

public class User : Entity, IAggregateRoot
{
    public UserProfile Profile { get; private set; }

    //Only for migrations
    #pragma warning disable CS8618
    protected User() { }
    #pragma warning restore CS8618

    public User(Guid id, UserProfile profile)
    {
        Id = id;
        Profile = profile;
    }

    public void SetNewProfile(UserProfile profile)
    {
        Profile = profile;
    }
}
