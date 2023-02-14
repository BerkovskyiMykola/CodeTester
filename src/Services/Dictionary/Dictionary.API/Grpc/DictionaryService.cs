using Dictionary.API.Infrastructure;
using Dictionary.API.Protos;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.API.Grpc;

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
        _logger.LogInformation($"Begin grpc call DictionaryService.GetDifficultyById for difficulty id {request.Id}.");

        var difficulty = await _context.Difficulties.FirstOrDefaultAsync(x => x.Id == request.Id);
        
        if (difficulty == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Difficulty with Id={request.Id} is not found."));
        }

        _logger.LogInformation($"Difficulty with Id={difficulty.Id} is found.");

        return new DifficultyResponse
        {
            Id = difficulty.Id,
            Name = difficulty.Name
        };
    }

    public override async Task<ProgrammingLanguageResponse> GetProgrammingLanguageById(ProgrammingLanguageIdRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Begin grpc call DictionaryService.GetProgrammingLanguageById for difficulty id {request.Id}.");

        var programmingLanguage = await _context.ProgrammingLanguages.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (programmingLanguage == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"ProgrammingLanguage with Id={request.Id} is not found."));
        }

        _logger.LogInformation($"ProgrammingLanguage with Id={programmingLanguage.Id} is found.");

        return new ProgrammingLanguageResponse
        {
            Id = programmingLanguage.Id,
            Name = programmingLanguage.Name
        };
    }

    public override async Task<TaskTypeResponse> GetTaskTypeById(TaskTypeIdRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Begin grpc call DictionaryService.GetTaskTypeById for difficulty id {request.Id}.");

        var taskType = await _context.TaskTypes.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (taskType == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"TaskType with Id={request.Id} is not found."));
        }

        _logger.LogInformation($"TaskType with Id={taskType.Id} is found.");

        return new TaskTypeResponse
        {
            Id = taskType.Id,
            Name = taskType.Name
        };
    }
}
