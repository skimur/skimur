namespace Skimur.Cassandra
{
    /// <summary>
    /// Retrieves the connection string for cassandra
    /// </summary>
    public interface ICassandraConnectionStringProvider
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
