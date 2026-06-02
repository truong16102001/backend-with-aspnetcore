using Dapper;
using System.Data;

namespace NetCore.DataAccess.Dapper
{
    public class ApplicationDbConnection
        : IApplicationDbConnection
    {
        private readonly IDbConnectionFactory _factory;

        public ApplicationDbConnection(
            IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<int> ExecuteAsync(
            string sql,
            object? param = null,
            IDbTransaction? transaction = null)
        {
            using var connection =
                _factory.CreateConnection();

            return await connection.ExecuteAsync(
                sql,
                param,
                transaction);
        }

        public async Task<List<T>> QueryAsync<T>(
            string sql,
            object? param = null,
            IDbTransaction? transaction = null)
        {
            using var connection =
                _factory.CreateConnection();

            var result =
                await connection.QueryAsync<T>(
                    sql,
                    param,
                    transaction);

            return result.ToList();
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(
            string sql,
            object? param = null,
            IDbTransaction? transaction = null)
        {
            using var connection =
                _factory.CreateConnection();

            return await connection
                .QueryFirstOrDefaultAsync<T>(
                    sql,
                    param,
                    transaction);
        }

        public async Task<T> QuerySingleAsync<T>(
            string sql,
            object? param = null,
            IDbTransaction? transaction = null)
        {
            using var connection =
                _factory.CreateConnection();

            return await connection.QuerySingleAsync<T>(
                sql,
                param,
                transaction);
        }

        public async Task<SqlMapper.GridReader>
            QueryMultipleAsync(
                string sql,
                object? param = null,
                IDbTransaction? transaction = null)
        {
            var connection =
                _factory.CreateConnection();

            return await connection.QueryMultipleAsync(
                sql,
                param,
                transaction);
        }
    }
}