using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace Infrastructure.Caching
{
    public class Cache : ICache
    {
        private readonly IRedisClientsManager _redisClientsManager;

        public Cache(IRedisClientsManager redisClientsManager)
        {
            _redisClientsManager = redisClientsManager;
        }

        public void Set<T>(string key, T value, TimeSpan expiresIn)
        {
            using (var client = _redisClientsManager.GetClient())
            {
                client.Set(key, value, expiresIn);
            }
        }

        public void Set<T>(string key, T value, DateTime expiresAt)
        {
            using (var client = _redisClientsManager.GetClient())
            {
                client.Set(key, value, expiresAt);
            }
        }

        public T GetAcquire<T>(string key, TimeSpan expiresIn, Func<T> acquire)
        {
            using (var client = _redisClientsManager.GetClient())
            {
                T result;

                var cached = client.Get<string>(key);
                if (cached == null)
                {
                    result = acquire();
                    client.Set(key, result, expiresIn);
                }
                else
                {
                    result = JsonSerializer.DeserializeFromString<T>(cached);
                }

                return result;
            }
        }

        public T GetAcquire<T>(string key, DateTime expiresAt, Func<T> acquire)
        {
            using (var client = _redisClientsManager.GetClient())
            {
                T result;

                var cached = client.Get<string>(key);
                if (cached == null)
                {
                    result = acquire();
                    client.Set(key, result, expiresAt);
                }
                else
                {
                    result = JsonSerializer.DeserializeFromString<T>(cached);
                }

                return result;
            }
        }
    }
}
