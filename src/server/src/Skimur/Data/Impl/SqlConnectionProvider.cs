using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Data.Impl
{
    public class SqlConnectionProvider : IDbConnectionProvider
    {
        IConnectionStringProvider _connectionStringProvider;

        public SqlConnectionProvider(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public async Task<IDbConnection> OpenConnection()
        {
            var connection = new Npgsql.NpgsqlConnection(_connectionStringProvider.ConnectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task Perform(Action<IDbConnection> action)
        {
            using (var connection = await OpenConnection())
                action(connection);
        }

        public async Task<T> Perform<T>(Func<IDbConnection, T> func)
        {
            using (var connection = await OpenConnection())
                return func(connection);
        }
    }
}
