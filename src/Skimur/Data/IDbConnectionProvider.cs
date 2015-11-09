using System;
using System.Data;

namespace Skimur.Data
{
    public interface IDbConnectionProvider
    {
        IDbConnection OpenConnection();

        void Perform(Action<IDbConnection> action);

        T Perform<T>(Func<IDbConnection, T> func);
    }
}
