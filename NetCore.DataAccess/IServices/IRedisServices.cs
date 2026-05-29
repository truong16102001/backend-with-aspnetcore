using NetCore.DataAccess.DataObject.DTOs.Redis;

namespace NetCore.DataAccess.IServices
{
    public interface IRedisServices
    {
        Task SetSessionAsync(CachedUserSession session, TimeSpan expiry); 

        Task<CachedUserSession?> GetSessionAsync(string sid); 

        Task RemoveSessionAsync(string sid); 

        Task SetRefreshTokenAsync(string hashRt, string sid, TimeSpan expiry); 

        Task<string?> GetSessionIdByRefreshTokenAsync(string hashRt); 

        Task RemoveRefreshTokenAsync(string hashRt); 

        Task AddUserSessionAsync(int userId, string sid, TimeSpan ttl); 

        Task<List<string>> GetUserSessionsAsync(int userId); 

        Task RemoveUserSessionAsync(int userId, string sid);
    }
}
