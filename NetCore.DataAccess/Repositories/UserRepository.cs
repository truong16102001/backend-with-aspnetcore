using NetCore.DataAccess.DataObject.Entities;
using NetCore.DataAccess.DBContext;
using NetCore.DataAccess.IRepositories;

namespace NetCore.DataAccess.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(MyDbContext dbContext)
           : base(dbContext)
        {
        }
    }
}
