using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Data.Impl
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly string _connectionString = null;

        public ConnectionStringProvider(IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("Postgres");

            if (connection == null) return;

            if (string.IsNullOrEmpty(connection))
                return;

            _connectionString = connection;
        }

        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    throw new Exception("No connection string found.");

                return _connectionString;
            }
        }
    }
}
