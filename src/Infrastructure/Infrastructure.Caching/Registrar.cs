using System.Collections.Generic;
using ServiceStack.Redis;
using SimpleInjector;
using Skimur;

namespace Infrastructure.Caching
{
    public class Registrar : IRegistrar
    {
        public void Register(Container container)
        {
            container.RegisterSingleton<ICache, Cache>();
            container.RegisterSingleton<IRedisClientsManager>(() =>
            {
                var readWrite = System.Configuration.ConfigurationManager.AppSettings["RedisReadWrite"];
                var read = System.Configuration.ConfigurationManager.AppSettings["RedisRead"];
                return new PooledRedisClientManager(readWrite.Split(';'), read.Split(';'));
            });
        }

        public int Order { get { return 0; } }
    }
}
