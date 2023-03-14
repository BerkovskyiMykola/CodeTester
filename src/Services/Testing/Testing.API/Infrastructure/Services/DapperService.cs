using Npgsql;
using System.Data;

namespace Testing.API.Infrastructure.Services;

public interface IDapperService
{
    public IDbConnection CreateConnection();
}

public class DapperService : IDapperService
{
    private readonly string _connectionString;

    public DapperService(IConfiguration configuration)
    {
        _connectionString = configuration["connectionString"]!;
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}
