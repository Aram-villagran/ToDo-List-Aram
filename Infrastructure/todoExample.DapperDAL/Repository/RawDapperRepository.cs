using Dapper;
using todoExample.DapperDAL.DbContext;
using todoExample.domain.Respository.Base;

namespace todoExample.DapperDAL.Repository;

public class RawDapperRepository : IRawDapperRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public RawDapperRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<T>> GenericQueryAsync<T>(string query, object? parameters = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<T>(query, parameters);
    }

    public async Task<dynamic> ExecuteAsync(string command, object parameters)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(command, parameters);
    }
}