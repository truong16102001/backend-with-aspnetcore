using NetCore.DataAccess.Common;
using NetCore.DataAccess.DataObject.DTOs.Redis;
using NetCore.DataAccess.IServices;
using StackExchange.Redis;
using System.Text.Json;
using static NetCore.DataAccess.Common.CONSTANT;

namespace NetCore.DataAccess.Services
{
    public class RedisServices : IRedisServices
    {
        private readonly StackExchange.Redis.IDatabase _database;
        private readonly IConnectionMultiplexer _redis;

        public RedisServices(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = redis.GetDatabase();
        }

        // =====================================================
        // GENERIC CACHE
        // =====================================================
        public async Task SetAsync<T>(string key, T data, TimeSpan expiry)
        {
            string json = JsonSerializer.Serialize(data);
            await _database.StringSetAsync(key, json, expiry);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(value!);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        // Dùng SCAN thay vì KEYS — non-blocking, production-safe
        public async Task RemoveByPrefixAsync(string prefix)
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{prefix}*").ToArray();
            if (keys.Length > 0)
                await _database.KeyDeleteAsync(keys);
        }

        // =====================================================
        // AUTH SESSION
        // =====================================================
        public async Task SetSessionAsync(CachedUserSession session, TimeSpan expiry)
        {
            await SetAsync(CONSTANT.REDIS.AUTH.SESSION(session.Sid), session, expiry);
        }

        public async Task<CachedUserSession?> GetSessionAsync(string sid)
        {
            return await GetAsync<CachedUserSession>(CONSTANT.REDIS.AUTH.SESSION(sid));
        }

        public async Task RemoveSessionAsync(string sid)
        {
            await RemoveAsync(CONSTANT.REDIS.AUTH.SESSION(sid));
        }

        // =====================================================
        // REFRESH TOKEN
        // =====================================================
        public async Task SetRefreshTokenAsync(string hashRt, string sid, TimeSpan expiry)
        {
            await _database.StringSetAsync(CONSTANT.REDIS.AUTH.REFRESH_TOKENS(hashRt), sid, expiry);
        }

        public async Task<string?> GetSessionIdByRefreshTokenAsync(string hashRt)
        {
            return await _database.StringGetAsync(CONSTANT.REDIS.AUTH.REFRESH_TOKENS(hashRt));
        }

        public async Task RemoveRefreshTokenAsync(string hashRt)
        {
            await RemoveAsync(CONSTANT.REDIS.AUTH.REFRESH_TOKENS(hashRt));
        }

        // =====================================================
        // USER SESSIONS
        // =====================================================
        public async Task AddUserSessionAsync(int userId, string sid, TimeSpan ttl)
        {
            string key = CONSTANT.REDIS.AUTH.USER_SESSIONS(userId);
            await _database.SetAddAsync(key, sid);
            await _database.KeyExpireAsync(key, ttl);
        }

        public async Task<List<string>> GetUserSessionsAsync(int userId)
        {
            var values = await _database.SetMembersAsync(CONSTANT.REDIS.AUTH.USER_SESSIONS(userId));
            return values.Select(x => x.ToString()).ToList();
        }

        public async Task RemoveUserSessionAsync(int userId, string sid)
        {
            await _database.SetRemoveAsync(CONSTANT.REDIS.AUTH.USER_SESSIONS(userId), sid);
        }
    }
}
