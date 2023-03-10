using Dapper;
using Npgsql;
using Testing.API.Application.Queries.Solutions.Models;

namespace Testing.API.Application.Queries.Solutions;

public interface ISolutionQueries
{
    Task<SolutionQueryModel> GetSolutionAsync(Guid id);

    Task<IEnumerable<SolutionQueryModel>> GetAllSolutionsAsync();
}

public class SolutionQueries : ISolutionQueries
{
    private string _connectionString = string.Empty;

    public SolutionQueries(IConfiguration configuration)
    {
        _connectionString = configuration["connectionString"]!;
    }

    public async Task<SolutionQueryModel> GetSolutionAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var result = await connection.QueryAsync<SolutionQueryModel>(
            @"select ""Id"" as Id, ""Value_Value"" as SolutionValue, ""Success"" as Success, ""TaskId"" as TaskId, ""UserId"" as UserId
                    FROM ""Solutions""
					WHERE ""Id"" = @id"
                , new { id }
            );

        if (result.AsList().Count == 0)
            throw new KeyNotFoundException();

        return result.ElementAt(0);
    }

    public async Task<IEnumerable<SolutionQueryModel>> GetAllSolutionsAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        return await connection.QueryAsync<SolutionQueryModel>(
            @"select ""Id"" as Id, ""Value_Value"" as SolutionValue, ""Success"" as Success, ""TaskId"" as TaskId, ""UserId"" as UserId
                    FROM ""Solutions"""
        );
    }
}
