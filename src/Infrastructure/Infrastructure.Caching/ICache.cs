using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Caching
{
    public interface ICache
    {
        void Set<T>(string key, T value, TimeSpan expiresIn);

        void Set<T>(string key, T value, DateTime expiresAt);

        T GetAcquire<T>(string key, TimeSpan expiresIn, Func<T> acquire);

        T GetAcquire<T>(string key, DateTime expiresAt, Func<T> acquire);
    }
}
