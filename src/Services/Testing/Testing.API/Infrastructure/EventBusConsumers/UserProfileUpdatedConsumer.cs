using EventBus.Messages.Events;
using MassTransit;
using Testing.Core.Domain.AggregatesModel.UserAggregate;
using Testing.Core.Domain.Repositories;

namespace StudentProfile.API.Infrastructure.EventBusConsumers;

public class UserProfileUpdatedConsumer : IConsumer<UserProfileUpdatedIntegrationEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserProfileUpdatedConsumer> _logger;

    public UserProfileUpdatedConsumer(
        IUserRepository userRepository,
        ILogger<UserProfileUpdatedConsumer> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserProfileUpdatedIntegrationEvent> context)
    {
        using (Serilog.Context.LogContext.PushProperty("IntegrationEventContext", $"{context.Message.Id}-{Program.AppName}"))
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                context.Message.Id,
                Program.AppName,
                context.Message);

            var createUserProfileResult = UserProfile.Create(
                context.Message.Lastname,
                context.Message.Firstname);

            if (!createUserProfileResult.Success)
            {
                _logger.LogInformation("----- Integration event failed - UserId: {UserId}", context.Message.UserId);
                return;
            }

            var user = await _userRepository.FindByIdAsync(context.Message.UserId);

            if (user == null)
            {
                _logger.LogInformation("----- Integration event failed - UserId: {UserId}", context.Message.UserId);
                return;
            }

            user.SetNewProfile(createUserProfileResult.Value!);

            await _userRepository.UnitOfWork.SaveEntitiesAsync();

            _logger.LogInformation("----- Integration event: {IntegrationEventId} at {AppName} suceeded - UserId: {UserId}",
                context.Message.Id,
                Program.AppName,
                context.Message.UserId);
        }
    }
}