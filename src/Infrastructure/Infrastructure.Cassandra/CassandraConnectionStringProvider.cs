using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Cassandra
{
    public class CassandraConnectionStringProvider : ICassandraConnectionStringProvider
    {
        #region Fields

        private readonly string _connectionString = null;

        #endregion

        #region Ctor
        
        public CassandraConnectionStringProvider()
        {
            var connection = System.Configuration.ConfigurationManager.AppSettings["Cassandra"];

            if (connection == null) return;

            if (string.IsNullOrEmpty(connection))
                return;

            _connectionString = connection == "registry" ? GetConnectionStringFromRegistry() : connection;
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

        #region Methods

        private string GetConnectionStringFromRegistry()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
