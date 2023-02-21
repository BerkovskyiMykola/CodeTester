using Dictionary.API.Protos;
using Grpc.Core;
using Testing.API.Infrastructure.Services.DictionaryService.DictionaryModels;

namespace Testing.API.Infrastructure.Services.DictionaryService;

public interface IDictionaryService
{
    Task<DifficultyModel?> GetDifficultyByIdAsync(int id);

    Task<ProgrammingLanguageModel?> GetProgrammingLanguageByIdAsync(int id);

    Task<TaskTypeModel?> GetTaskTypeByIdAsync(int id);
}

public class DictionaryService : IDictionaryService
{
    private readonly DictionaryGrpc.DictionaryGrpcClient _dictionaryClient;
    private readonly ILogger<DictionaryService> _logger;

    public DictionaryService(
        DictionaryGrpc.DictionaryGrpcClient dictionaryClient,
        ILogger<DictionaryService> logger)
    {
        _dictionaryClient = dictionaryClient;
        _logger = logger;
    }

    public async Task<DifficultyModel?> GetDifficultyByIdAsync(int id)
    {
        try
        {
            var response = await _dictionaryClient.GetDifficultyByIdAsync(new DifficultyIdRequest { Id = id });

            _logger.LogDebug("grpc response {@response}", response);

            return new DifficultyModel
            {
                Id = response.Id,
                Name = response.Name
            };
        }
        catch (RpcException)
        {
            return default;
        }
    }

    public async Task<TaskTypeModel?> GetTaskTypeByIdAsync(int id)
    {
        try
        {
            var response = await _dictionaryClient.GetTaskTypeByIdAsync(new TaskTypeIdRequest { Id = id });

            _logger.LogDebug("grpc response {@response}", response);

            return new TaskTypeModel
            {
                Id = response.Id,
                Name = response.Name
            };
        }
        catch (RpcException)
        {
            return default;
        }
    }

    public async Task<ProgrammingLanguageModel?> GetProgrammingLanguageByIdAsync(int id)
    {
        try
        {
            var response = await _dictionaryClient.GetProgrammingLanguageByIdAsync(new ProgrammingLanguageIdRequest { Id = id });

            _logger.LogDebug("grpc response {@response}", response);

            return new ProgrammingLanguageModel
            {
                Id = response.Id,
                Name = response.Name
            };
        }
        catch (RpcException)
        {
            return default;
        }
    }
}
