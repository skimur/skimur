using Microsoft.Extensions.Configuration;

namespace Skimur.Cassandra
{
    public class CassandraConnectionStringProvider : ICassandraConnectionStringProvider
    {
        #region Fields

        private readonly string _connectionString = null;

        #endregion

        #region Ctor

        public CassandraConnectionStringProvider(IConfiguration configuration)
        {
            var connection = configuration.Get<string>("Data:Cassandra", null);

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
