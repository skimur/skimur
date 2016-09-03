using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Data
{
    public interface IDbConnectionProvider
    {
        Task<IDbConnection> OpenConnection();

        Task Perform(Action<IDbConnection> action);

        Task<T> Perform<T>(Func<IDbConnection, T> func);

        Task<T> Perform<T>(Func<IDbConnection, Task<T>> func);
    }
}
