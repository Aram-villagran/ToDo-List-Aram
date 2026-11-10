using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace todoExample.DapperDAL.DbContext;

public class ConnectionFactory(IConfiguration configuration) : IConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        try
        {
            var connectionString = configuration.GetConnectionString("DefaultMySQLConnection");
            return new MySqlConnection(connectionString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new Exception();
        }
    }
}