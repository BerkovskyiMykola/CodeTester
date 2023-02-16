namespace EventBus.Messages.Events;

public record UserProfileUpdatedIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; private init; }
    public string Firstname { get; private init; }
    public string Lastname { get; private init; }

    public UserProfileUpdatedIntegrationEvent(
        Guid userId,
        string firstname,
        string lastname)
    {
        UserId = userId;
        Firstname = firstname;
        Lastname = lastname;
    }
}
