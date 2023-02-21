using Dictionary.API.Protos;
using Grpc.Core;
using Testing.API.Infrastructure.Services.DictionaryData;

namespace Testing.API.Infrastructure.Services;

public interface IDictionaryService
{
    Task<DifficultyData?> GetDifficultyByIdAsync(int id);

    Task<ProgrammingLanguageData?> GetProgrammingLanguageByIdAsync(int id);

    Task<TaskTypeData?> GetTaskTypeByIdAsync(int id);
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

    public async Task<DifficultyData?> GetDifficultyByIdAsync(int id)
    {
        try
        {
            var response = await _dictionaryClient.GetDifficultyByIdAsync(new DifficultyIdRequest { Id = id });

            _logger.LogDebug("grpc response {@response}", response);

            return new DifficultyData
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

    public async Task<TaskTypeData?> GetTaskTypeByIdAsync(int id)
    {
        try
        {
            var response = await _dictionaryClient.GetTaskTypeByIdAsync(new TaskTypeIdRequest { Id = id });

            _logger.LogDebug("grpc response {@response}", response);

            return new TaskTypeData
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

    public async Task<ProgrammingLanguageData?> GetProgrammingLanguageByIdAsync(int id)
    {
        try
        {
            var response = await _dictionaryClient.GetProgrammingLanguageByIdAsync(new ProgrammingLanguageIdRequest { Id = id });

            _logger.LogDebug("grpc response {@response}", response);

            return new ProgrammingLanguageData
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
