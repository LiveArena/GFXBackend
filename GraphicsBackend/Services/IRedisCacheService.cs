using StackExchange.Redis;
using System.Text.Json;

namespace GraphicsBackend.Services
{
    public interface IRedisCacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task SetMultipleAsync<T>(Dictionary<string, T> items, TimeSpan? expiry = null);
        Task<T> GetAsync<T>(string key);
        Task<Dictionary<string, T>> GetMultipleAsync<T>(IEnumerable<string> keys);
        Task RemoveAsync(string key);
    }
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var jsonData = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, jsonData, expiry);
        }
        public async Task SetMultipleAsync<T>(Dictionary<string, T> items, TimeSpan? expiry = null)
        {
            var db = _connectionMultiplexer.GetDatabase();
            
            var redisKeyValues = items.Select(item =>
                new KeyValuePair<RedisKey, RedisValue>(item.Key, JsonSerializer.Serialize(item.Value))).ToArray();
            
            await db.StringSetAsync(redisKeyValues);
           
            if (expiry.HasValue)
            {
                foreach (var item in items.Keys)
                {
                    await db.KeyExpireAsync(item, expiry);
                }
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var jsonData = await db.StringGetAsync(key);

            if (jsonData.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(jsonData);
        }
        public async Task<Dictionary<string, T>> GetMultipleAsync<T>(IEnumerable<string> keys)
        {
            var db = _connectionMultiplexer.GetDatabase();
            
            RedisKey[] redisKeys = keys.Select(k => (RedisKey)k).ToArray();
            
            RedisValue[] redisValues = await db.StringGetAsync(redisKeys);

            var result = new Dictionary<string, T>();

            for (int i = 0; i < redisKeys.Length; i++)
            {                
                if (!redisValues[i].IsNull)
                {
                    result[redisKeys[i]] = JsonSerializer.Deserialize<T>(redisValues[i]);
                }
            }

            return result;
        }
        

        public async Task RemoveAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
    }
}
