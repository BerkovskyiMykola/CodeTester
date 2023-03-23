using Dapper;
using Testing.API.Application.Queries.Solutions.Models;
using Testing.API.Infrastructure.Services;

namespace Testing.API.Application.Queries.Solutions;

public interface ISolutionQueries
{
    Task<IEnumerable<SolutionQueryModel>> GetSolutionsByUserIdAndTaskIdAsync(Guid userId, Guid taskId);
    Task<SolutionQueryModel> GetLastSuccessfulSolutionByUserIdAndTaskIdAsync(Guid userId, Guid taskId);
}

public class SolutionQueries : ISolutionQueries
{
    private readonly IDapperService _dapperService;

    public SolutionQueries(IDapperService dapperService)
    {
        _dapperService = dapperService;
    }

    public async Task<IEnumerable<SolutionQueryModel>> GetSolutionsByUserIdAndTaskIdAsync(Guid userId, Guid taskId)
    {
        using var connection = _dapperService.CreateConnection();

        var query =
            @$"SELECT ""Id"",
            ""Value_Value"", ""Success"",
            ""TaskId"", ""UserId""
            FROM ""Solutions""
            WHERE ""UserId"" = '{userId}' AND ""TaskId"" = '{taskId}'
            ORDER BY ""CreateDate"" DESC;";

        var result = await connection.QueryAsync<dynamic>(query);

        return result.Select(MapForListToSolutionQueryModel).ToList();
    }

    public async Task<SolutionQueryModel> GetLastSuccessfulSolutionByUserIdAndTaskIdAsync(Guid userId, Guid taskId)
    {
        using var connection = _dapperService.CreateConnection();

        var query =
            @$"SELECT ""Id"",
            ""Value_Value"", ""Success"",
            ""TaskId"", ""UserId""
            FROM ""Solutions""
            WHERE ""UserId"" = '{userId}' AND ""TaskId"" = '{taskId}' AND ""Success"" IS TRUE
            ORDER BY ""CreateDate"" DESC
            FETCH FIRST 1 ROWS ONLY;";


        var result = await connection.QueryAsync<dynamic>(query);

        if (result.AsList().Count == 0)
            throw new KeyNotFoundException();

        return MapToSolutionQueryModel(result);
    }

    private SolutionQueryModel MapToSolutionQueryModel(dynamic obj)
    {
        return new SolutionQueryModel()
        {
            Id = obj[0].Id,
            UserId = obj[0].UserId,
            TaskId = obj[0].TaskId,
            Success = obj[0].Success,
            SolutionValue = obj[0].Value_Value,
        };
    }

    private SolutionQueryModel MapForListToSolutionQueryModel(dynamic obj)
    {
        return new SolutionQueryModel()
        {
            Id = obj.Id,
            UserId = obj.UserId,
            TaskId = obj.TaskId,
            Success = obj.Success,
            SolutionValue = obj.Value_Value,
        };
    }
}
