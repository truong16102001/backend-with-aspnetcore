using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetCore.DataAccess.Common;
using System.Data;

namespace NetCore.DataAccess.Dapper
{
    public class SqlServerConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public SqlServerConnectionFactory(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("ConnStr"));
        }
    }
}
