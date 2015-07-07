using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Infrastructure.Data
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
        public ConnectionStringProvider()
        {
            var connection = System.Configuration.ConfigurationManager.ConnectionStrings["Skimur"];

            if(connection == null) return;

            if (string.IsNullOrEmpty(connection.ConnectionString))
                return;

            _connectionString = connection.ConnectionString == "registry" ? GetConnectionStringFromRegistry() : connection.ConnectionString;
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
