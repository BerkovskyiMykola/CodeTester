using Dapper;
using Testing.API.Application.Queries.Solutions.Models;
using Testing.API.Application.Queries.Tasks.Models;
using Testing.API.Infrastructure.Services;
using Testing.Core.Domain.AggregatesModel.SolutionAggregate;

namespace Testing.API.Application.Queries.Solutions;

public interface ISolutionQueries
{
    Task<SolutionQueryModel> GetSolutionByUserIdAndTaskIdAsync(Guid userId, Guid taskId);
}

public class SolutionQueries : ISolutionQueries
{
    private readonly IDapperService _dapperService;

    public SolutionQueries(IDapperService dapperService)
    {
        _dapperService = dapperService;
    }

    public async Task<SolutionQueryModel> GetSolutionByUserIdAndTaskIdAsync(Guid userId, Guid taskId)
    {
        using var connection = _dapperService.CreateConnection();

        var query =
            @$"SELECT ""Id"",
            ""Value_Value"", ""Success"",
            ""TaskId"", ""UserId""
            FROM ""Solutions""
            WHERE ""UserId"" = '{userId}' AND ""TaskId"" = '{taskId}';";


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
}
