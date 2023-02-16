using Dictionary.API.Protos;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.API.Infrastructure.GrpcServices;

public class DictionaryService : DictionaryGrpc.DictionaryGrpcBase
{
    private readonly DictionaryContext _context;
    private readonly ILogger<DictionaryService> _logger;

    public DictionaryService(
        DictionaryContext context,
        ILogger<DictionaryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public override async Task<DifficultyResponse> GetDifficultyById(DifficultyIdRequest request, ServerCallContext context)
    {
        var difficulty = await _context.Difficulties
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (difficulty == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Difficulty with Id={request.Id} is not found."));
        }

        var response = new DifficultyResponse
        {
            Id = difficulty.Id,
            Name = difficulty.Name
        };

        _logger.LogInformation("Sending response {@response}.", response);

        return response;
    }

    public override async Task<ProgrammingLanguageResponse> GetProgrammingLanguageById(ProgrammingLanguageIdRequest request, ServerCallContext context)
    {
        var programmingLanguage = await _context.ProgrammingLanguages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (programmingLanguage == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"ProgrammingLanguage with Id={request.Id} is not found."));
        }

        var response = new ProgrammingLanguageResponse
        {
            Id = programmingLanguage.Id,
            Name = programmingLanguage.Name
        };

        _logger.LogInformation("Sending response {@response}.", response);

        return response;
    }

    public override async Task<TaskTypeResponse> GetTaskTypeById(TaskTypeIdRequest request, ServerCallContext context)
    {
        var taskType = await _context.TaskTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (taskType == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"TaskType with Id={request.Id} is not found."));
        }

        var response = new TaskTypeResponse
        {
            Id = taskType.Id,
            Name = taskType.Name
        };

        _logger.LogInformation("Sending response {@response}.", response);

        return response;
    }
}
