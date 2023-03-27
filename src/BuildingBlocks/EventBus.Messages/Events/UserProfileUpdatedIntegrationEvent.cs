namespace EventBus.Messages.Events;

public record UserProfileUpdatedIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; private init; }
    public string Fullname { get; private init; }

    public UserProfileUpdatedIntegrationEvent(
        Guid userId,
        string fullname)
    {
        UserId = userId;
        Fullname = fullname;
    }
}
