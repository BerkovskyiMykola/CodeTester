namespace EventBus.Messages.Events;

public record UserProfileCreatedIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; private init; }
    public string Fullname { get; private init; }

    public UserProfileCreatedIntegrationEvent(
        Guid userId,
        string fullname)
    {
        UserId = userId;
        Fullname = fullname;
    }
}
