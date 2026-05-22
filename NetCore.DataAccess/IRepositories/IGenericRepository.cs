using System.Linq.Expressions;

namespace NetCore.DataAccess.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> Query();

        Task<List<T>> GetAll();

        Task<T?> GetById(int id);

        Task<List<T>> Find(Expression<Func<T, bool>> predicate);

        Task Insert(T entity);

        void Update(T entity);

        void Delete(T entity);

    }
}
