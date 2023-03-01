namespace EventBus.Messages.Events;

public record UserProfileCreatedIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; private init; }
    public string Firstname { get; private init; }
    public string Lastname { get; private init; }

    public UserProfileCreatedIntegrationEvent(
        Guid userId,
        string firstname,
        string lastname)
    {
        UserId = userId;
        Firstname = firstname;
        Lastname = lastname;
    }
}
