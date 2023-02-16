using EventBus.Messages.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Testing.Core.Domain.AggregatesModel.SolutionAggregate;
using Testing.Core.Domain.Repositories;
using Testing.Infrastructure.Persistence;

namespace StudentProfile.API.Infrastructure.EventBusConsumers;

public class UserUpdatedConsumer : IConsumer<UserUpdatedIntegrationEvent>
{
    private readonly TestingContext _context;
    private readonly ILogger<UserUpdatedConsumer> _logger;
    private readonly ISolutionRepository _solutionRepository;

    public UserUpdatedConsumer(
        TestingContext context,
        ILogger<UserUpdatedConsumer> logger,
        ISolutionRepository solutionRepository)
    {
        _context = context;
        _logger = logger;
        _solutionRepository = solutionRepository;
    }

    public async Task Consume(ConsumeContext<UserUpdatedIntegrationEvent> context)
    {
        var createUserResult = User.Create(
            context.Message.UserId, 
            context.Message.Email,
            context.Message.Lastname, 
            context.Message.Firstname);

        if (!createUserResult.Success)
            return;

        var solutions = await _context.Solutions
            .Where(x => x.User.Id == context.Message.UserId)
            .ToListAsync();

        foreach (var solution in solutions)
        {
            var updatedSolution = new Solution(
                solution.Id,
                solution.TaskId,
                createUserResult.Value!,
                solution.Value,
                solution.Success);

            _solutionRepository.Upsert(updatedSolution);
        }

        await _solutionRepository.UnitOfWork.SaveEntitiesAsync();
    }
}
