using Testing.Core.Bases;

namespace Testing.Core.Domain.AggregatesModel.UserAggregate;

public class User : Entity, IAggregateRoot
{
    public Email Email { get; private set; }
    public UserProfile Profile { get; private set; }

    //Only for migrations
    #pragma warning disable CS8618
    protected User() { }
    #pragma warning restore CS8618

    public User(Guid id, Email email, UserProfile profile)
    {
        Id = id;
        Email = email;
        Profile = profile;
    }

    public void SetNewProfile(UserProfile profile)
    {
        Profile = profile;
    }
}
