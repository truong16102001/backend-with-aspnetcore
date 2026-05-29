using Microsoft.EntityFrameworkCore;
using NetCore.DataAccess.DataObject.Entities;
using NetCore.DataAccess.DBContext;
using NetCore.DataAccess.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.Repositories
{
    public class UserSessionRepository : GenericRepository<UserSession>, IUserSessionRepository
    {
        private readonly MyDbContext _context;
        public UserSessionRepository(MyDbContext dbContext)
           : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<UserSession?> GetValidSessionByHashRtAsync(string hashRt)
        {
            return await _context.UserSessions
               .FirstOrDefaultAsync(x =>
                   x.RefreshTokenHash == hashRt
                   && !x.IsRevoked
                   && x.ExpiredAt > DateTime.UtcNow);
        }

        /// <summary>
        /// Get valid session by sid
        ///
        /// Conditions:
        /// - sid matched
        /// - not revoked
        /// - refresh token not expired
        /// </summary>
        public async Task<UserSession?>
            GetValidSessionBySidAsync(
                string sid)
        {
            return await _context.UserSessions
                .FirstOrDefaultAsync(x =>
                    x.Sid == sid
                    && !x.IsRevoked
                    && x.ExpiredAt > DateTime.UtcNow);
        }

        public async Task<List<UserSession>> GetAllValidSessionsByUserIdAsync(int userId)
        {
            return await _context.UserSessions
                .Where(x =>
                    x.UserID == userId &&
                    !x.IsRevoked &&
                    x.ExpiredAt > DateTime.UtcNow)
                .ToListAsync();
        }
    }
}
