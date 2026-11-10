using System.Data;

namespace todoExample.DapperDAL.DbContext;

public interface IConnectionFactory
{
    public IDbConnection CreateConnection();
}