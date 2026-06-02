using System.Data;

namespace NetCore.DataAccess.Dapper
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
