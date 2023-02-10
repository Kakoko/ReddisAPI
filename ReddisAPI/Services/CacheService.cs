using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System.Text.Json;

namespace ReddisAPI.Services
{
    public class CacheService : ICacheService
    {

        private StackExchange.Redis.IDatabase _cacheDb;
        public CacheService()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _cacheDb = redis.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var values = _cacheDb.StringGet(key);
            if (!string.IsNullOrEmpty(values))
            {
                return JsonSerializer.Deserialize<T>(values);
            }

            return default;
        }

        public object RemoveData(string key)
        {
            var exists = _cacheDb.KeyExists(key);

            if(exists)
            {
                return _cacheDb.KeyDelete(key); 
            }

            return false;

        }

        public bool SetData<T>(string key, T data, DateTimeOffset expiration)
        {
            var expiryTime = expiration.DateTime.Subtract(DateTime.Now);

            return _cacheDb.StringSet(key, JsonSerializer.Serialize(data), expiryTime);

            
        }
    }
}
