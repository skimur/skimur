using Microsoft.Extensions.Configuration;
using System;

namespace Skimur.Data
{
    /// <summary>
    /// Retrieves the sql connection string
    /// </summary>
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        #region Fields

        private readonly string _connectionString = null;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringProvider"/> class.
        /// </summary>
        public ConnectionStringProvider(IConfiguration configuration)
        {
            var connection = configuration.Get<string>("Data:Postgres", null);

            if (connection == null) return;

            if (string.IsNullOrEmpty(connection))
                return;

            _connectionString = connection;
        }

        #endregion

        #region IConnectionStringProvider

        /// <summary>
        /// Is there a valid connection string configured?
        /// </summary>
        public bool HasConnectionString
        {
            get { return !string.IsNullOrEmpty(_connectionString); }
        }

        /// <summary>
        /// The connection string
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
        }

        #endregion
    }
}
