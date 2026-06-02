using Dapper;
using System.Data;

namespace NetCore.DataAccess.Dapper
{
    public interface IApplicationDbConnection
    {
        Task<int> ExecuteAsync(
            string sql,
            object? param = null,
            IDbTransaction? transaction = null);

        Task<List<T>> QueryAsync<T>(
            string sql,
            object? param = null,
            IDbTransaction? transaction = null);

        Task<T?> QueryFirstOrDefaultAsync<T>(
            string sql,
            object? param = null,
            IDbTransaction? transaction = null);

        Task<T> QuerySingleAsync<T>(
            string sql,
            object? param = null,
            IDbTransaction? transaction = null);

        Task<SqlMapper.GridReader> QueryMultipleAsync(
            string sql,
            object? param = null,
            IDbTransaction? transaction = null);
    }
}