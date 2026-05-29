using NetCore.DataAccess.DataObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.IRepositories
{
    public interface IUserSessionRepository : IGenericRepository<UserSession>
    {
        Task<UserSession?> GetValidSessionBySidAsync(string sid);

        Task<UserSession?> GetValidSessionByHashRtAsync(string hashRt);

        Task<List<UserSession>> GetAllValidSessionsByUserIdAsync(int userId);
    }
}
