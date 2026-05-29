using Microsoft.EntityFrameworkCore;
using NetCore.DataAccess.DataObject.Entities;
using NetCore.DataAccess.DBContext;
using NetCore.DataAccess.IRepositories;

namespace NetCore.DataAccess.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly MyDbContext _context;
        public UserRepository(MyDbContext dbContext)
           : base(dbContext)
        {
            _context = dbContext;
        }

        /// <summary>
        /// Get user with permissions
        ///
        /// Include:
        /// - UserPermissions
        /// - Feature
        /// - Permission
        /// </summary>
        public async Task<User?>
            GetUserWithPermissionsAsync(
                int userId)
        {
            return await _context.Users
                .Include(x => x.UserPermissions)
                    .ThenInclude(x => x.Feature)

                .Include(x => x.UserPermissions)
                    .ThenInclude(x => x.Permission)

                .FirstOrDefaultAsync(x =>
                    x.UserID == userId);
        }
    }
}
