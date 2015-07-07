using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    /// <summary>
    /// Retrieves the sql connection string
    /// </summary>
    public interface IConnectionStringProvider
    {
        /// <summary>
        /// Is there a valid connection string configured?
        /// </summary>
        bool HasConnectionString { get; }

        /// <summary>
        /// The connection string
        /// </summary>
        string ConnectionString { get; }
    }
}
