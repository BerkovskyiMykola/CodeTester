using Dictionary.API.Protos;
using Grpc.Core;
using Testing.API.DTOs.DictionaryData;

namespace Testing.API.Services;

public class DictionaryService : IDictionaryService
{
    private readonly DictionaryGrpc.DictionaryGrpcClient _dictionaryClient;
    private readonly ILogger<DictionaryService> _logger;

    public DictionaryService(DictionaryGrpc.DictionaryGrpcClient dictionaryClient, ILogger<DictionaryService> logger)
    {
        _dictionaryClient = dictionaryClient;
        _logger = logger;
    }

    public async Task<DifficultyData?> GetDifficultyByIdAsync(int id)
    {
        _logger.LogDebug("grpc client created, difficulty request = {@id}", id);

        try
        {
            var response = await _dictionaryClient.GetDifficultyByIdAsync(new DifficultyIdRequest { Id = id });
            _logger.LogDebug("grpc response {@response}", response);

            return MapToDifficultyData(response);
        }
        catch (RpcException e)
        {
            _logger.LogError("Error calling via grpc: {Status} - {Message}", e.Status, e.Message);
            return default;
        }
    }

    public async Task<TaskTypeData?> GetTaskTypeByIdAsync(int id)
    {
        _logger.LogDebug("grpc client created, task type request = {@id}", id);

        try
        {
            var response = await _dictionaryClient.GetTaskTypeByIdAsync(new TaskTypeIdRequest { Id = id });
            _logger.LogDebug("grpc response {@response}", response);

            return MapToTaskTypeData(response);
        }
        catch (RpcException e)
        {
            _logger.LogError("Error calling via grpc: {Status} - {Message}", e.Status, e.Message);
            return default;
        }
    }

    public async Task<ProgrammingLanguageData?> GetProgrammingLanguageByIdAsync(int id)
    {
        _logger.LogDebug("grpc client created, programming language request = {@id}", id);

        try
        {
            var response = await _dictionaryClient.GetProgrammingLanguageByIdAsync(new ProgrammingLanguageIdRequest { Id = id });
            _logger.LogDebug("grpc response {@response}", response);

            return MapToProgrammingLanguageData(response);
        }
        catch (RpcException e)
        {
            _logger.LogError("Error calling via grpc: {Status} - {Message}", e.Status, e.Message);
            return default;
        }
    }

    private DifficultyData? MapToDifficultyData(DifficultyResponse difficultyResponse)
    {
        if (difficultyResponse == null)
        {
            return null;
        }

        var map = new DifficultyData
        {
            Id = difficultyResponse.Id,
            Name = difficultyResponse.Name
        };

        return map;
    }

    private TaskTypeData? MapToTaskTypeData(TaskTypeResponse taskTypeResponse)
    {
        if (taskTypeResponse == null)
        {
            return null;
        }

        var map = new TaskTypeData
        {
            Id = taskTypeResponse.Id,
            Name = taskTypeResponse.Name
        };

        return map;
    }

    private ProgrammingLanguageData? MapToProgrammingLanguageData(ProgrammingLanguageResponse programmingLanguageResponse)
    {
        if (programmingLanguageResponse == null)
        {
            return null;
        }

        var map = new ProgrammingLanguageData
        {
            Id = programmingLanguageResponse.Id,
            Name = programmingLanguageResponse.Name
        };

        return map;
    }
}
