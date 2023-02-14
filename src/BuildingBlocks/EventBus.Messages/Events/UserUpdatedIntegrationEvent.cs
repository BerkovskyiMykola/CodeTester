namespace EventBus.Messages.Events;

public record UserUpdatedIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; private init; }
    public string Email { get; private init; }
    public string Firstname { get; private init; }
    public string Lastname { get; private init; }

    public UserUpdatedIntegrationEvent(
        Guid userId,
        string email,
        string firstname,
        string lastname)
    {
        UserId = userId;
        Email = email;
        Firstname = firstname;
        Lastname = lastname;
    }
}
