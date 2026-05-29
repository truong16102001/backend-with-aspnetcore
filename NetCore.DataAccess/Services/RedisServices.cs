using NetCore.DataAccess.Common;
using NetCore.DataAccess.DataObject.DTOs.Redis;
using NetCore.DataAccess.IServices;
using StackExchange.Redis;
using System.Text.Json;

namespace NetCore.DataAccess.Services
{
    public class RedisServices : IRedisServices { 
        private readonly StackExchange.Redis.IDatabase _database; 

        public RedisServices(IConnectionMultiplexer redis) 
        { 
            _database = redis.GetDatabase(); 
        } 

        public async Task SetSessionAsync(CachedUserSession session, TimeSpan expiry) 
        { 
            string json = JsonSerializer.Serialize(session); 
            await _database.StringSetAsync(CONSTANT.REDIS.AUTH.SESSION(session.Sid!), json, expiry); 
        } 

        public async Task<CachedUserSession?> GetSessionAsync(string sid) {
            var value = await _database.StringGetAsync(CONSTANT.REDIS.AUTH.SESSION(sid)); 
            if (value.IsNullOrEmpty) { return null; } 
            return JsonSerializer.Deserialize<CachedUserSession>(value!); 
        } 

        public async Task RemoveSessionAsync(string sid) { 
            await _database.KeyDeleteAsync(CONSTANT.REDIS.AUTH.SESSION(sid)); 
        } 

        public async Task SetRefreshTokenAsync(string hashRt, string sid, TimeSpan expiry) { 
            await _database.StringSetAsync(CONSTANT.REDIS.AUTH.REFRESH_TOKENS(hashRt), sid, expiry); 
        } 

        public async Task<string?> GetSessionIdByRefreshTokenAsync(string hashRt) { 
            return await _database.StringGetAsync(CONSTANT.REDIS.AUTH.REFRESH_TOKENS(hashRt));
        } 

        public async Task RemoveRefreshTokenAsync(string hashRt) { 
            await _database.KeyDeleteAsync(CONSTANT.REDIS.AUTH.REFRESH_TOKENS(hashRt)); 
        }

        public async Task AddUserSessionAsync(int userId, string sid, TimeSpan expiry) 
        { 
            string key = CONSTANT.REDIS.AUTH.USER_SESSIONS(userId);
            await _database.SetAddAsync(key, sid);
            await _database.KeyExpireAsync(key, expiry);
        } 

        public async Task<List<string>> GetUserSessionsAsync(int userId) { 
            var values = await _database.SetMembersAsync(CONSTANT.REDIS.AUTH.USER_SESSIONS(userId)); 
            return values.Select(x => x.ToString()).ToList(); 
        } 

        public async Task RemoveUserSessionAsync(int userId, string sid) { 
            await _database.SetRemoveAsync(CONSTANT.REDIS.AUTH.USER_SESSIONS(userId), sid); 
        } 

    }
}
