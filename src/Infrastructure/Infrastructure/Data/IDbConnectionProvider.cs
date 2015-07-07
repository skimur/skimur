using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public interface IDbConnectionProvider
    {
        IDbConnection OpenConnection();

        void Perform(Action<IDbConnection> action);

        T Perform<T>(Func<IDbConnection, T> func);
    }
}
