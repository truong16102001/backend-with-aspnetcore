using NetCore.DataAccess.DataObject.DTOs.Redis;

namespace NetCore.DataAccess.IServices
{
    public interface IRedisServices
    {
        // =====================================================
        // GENERIC CACHE
        // =====================================================
        Task SetAsync<T>(string key, T data, TimeSpan expiry);

        Task<T?> GetAsync<T>(string key);

        Task RemoveAsync(string key);

        Task RemoveByPrefixAsync(string prefix);

        // =====================================================
        // AUTH SESSION
        // =====================================================
        Task SetSessionAsync(CachedUserSession session, TimeSpan expiry);

        Task<CachedUserSession?> GetSessionAsync(string sid);

        // =====================================================
        // REFRESH TOKEN
        // =====================================================
        Task SetRefreshTokenAsync(string hashRt, string sid, TimeSpan expiry);

        Task<string?> GetSessionIdByRefreshTokenAsync(string hashRt);

        Task RemoveRefreshTokenAsync(string hashRt);

        // =====================================================
        // USER SESSIONS
        // =====================================================
        Task AddUserSessionAsync(int userId, string sid, TimeSpan ttl);

        Task<List<string>> GetUserSessionsAsync(int userId);

        Task RemoveUserSessionAsync(int userId, string sid);

        Task RemoveSessionAsync(string sid);
        
    }
}
