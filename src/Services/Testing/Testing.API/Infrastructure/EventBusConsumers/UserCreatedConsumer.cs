﻿using EventBus.Messages.Events;
using MassTransit;
using Testing.Core.Domain.AggregatesModel.UserAggregate;
using Testing.Core.Domain.Repositories;

namespace Testing.API.Infrastructure.EventBusConsumers;

public class UserCreatedConsumer : IConsumer<UserCreatedIntegrationEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(
        IUserRepository userRepository,
        ILogger<UserCreatedConsumer> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
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

            var createUserEmailResult = Email.Create(
                context.Message.Email);

            if (!createUserEmailResult.Success)
            {
                _logger.LogInformation("----- Integration event failed - UserId: {UserId}", context.Message.UserId);
                return;
            }

            var user = new User(context.Message.UserId, createUserEmailResult.Value!, createUserProfileResult.Value!);

            _userRepository.Add(user);

            await _userRepository.UnitOfWork.SaveEntitiesAsync();

            _logger.LogInformation("----- Integration event: {IntegrationEventId} at {AppName} suceeded - UserId: {UserId}",
                context.Message.Id,
                Program.AppName,
                context.Message.UserId);
        }
    }
}