using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
