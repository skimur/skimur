using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;

namespace Infrastructure.Data
{
    public class SqlConnectionProvider : IDbConnectionProvider
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly DbProviderFactory _factory;

        public SqlConnectionProvider(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
            LicenseUtils.RegisterLicense("2283-e1JlZjoyMjgzLE5hbWU6TWVkWENoYW5nZSxUeXBlOkluZGllLEhhc2g6TU" +
                "FyaTVzNGdQcEdlc0pqd1ZIUXVlL0lacDBZcCt3TkFLY0UyMTlJblBuMzRLNWFRb" +
                "HBYN204aGkrQXlRYzUvZnNVUlZzWXd4NjR0OFlXZEpjNUNYRTdnMjBLR0ZjQmhG" +
                "dTFNMHZVazJqcHdQb1RrbStDaHNPRm11Qm50TnZzOTkwcHAzRkxtTC9idThMekN" +
                "lTVRndFBORzBuREZ0WGJUdzdRMi80K09lQ2tZPSxFeHBpcnk6MjAxNi0wMi0xOX" +
                "0=");

            OrmLiteConfig.DialectProvider = PostgreSqlDialect.Provider;
            //OrmLiteConfig.DialectProvider.NamingStrategy = new NamingStrategy();
            _factory = Npgsql.NpgsqlFactory.Instance;
        }

        public IDbConnection OpenConnection()
        {
            if (!_connectionStringProvider.HasConnectionString)
                throw new Exception("There is no connection string configured!");

            var connection = _factory.CreateConnection();
            if (connection == null) throw new Exception("Couldn't create the connection from the factory.");
            connection.ConnectionString = _connectionStringProvider.ConnectionString;
            connection.Open();
#if WRAP_CONNECTION
            return new Con(connection);
#else
            return connection;
#endif
        }

        public void Perform(Action<IDbConnection> action)
        {
            using (var conn = OpenConnection())
                action(conn);
        }

        public T Perform<T>(Func<IDbConnection, T> func)
        {
            using (var conn = OpenConnection())
                return func(conn);
        }

        class NamingStrategy : OrmLiteNamingStrategyBase
        {
            public override string GetTableName(string name)
            {
                return name;
            }

            public override string GetColumnName(string name)
            {
                return name;
            }
        }

#if WRAP_CONNECTION

        class Con : IDbConnection
        {
            private readonly IDbConnection _con;

            public Con(IDbConnection con)
            {
                _con = con;
            }

            public IDbTransaction BeginTransaction(IsolationLevel il)
            {
                return _con.BeginTransaction(il);
            }

            public IDbTransaction BeginTransaction()
            {
                return _con.BeginTransaction();
            }

            public void ChangeDatabase(string databaseName)
            {
                _con.ChangeDatabase(databaseName);
            }

            public void Close()
            {
                _con.Close();
            }

            public string ConnectionString
            {
                get { return _con.ConnectionString; }
                set { _con.ConnectionString = value; }
            }

            public int ConnectionTimeout
            {
                get { return _con.ConnectionTimeout; }
            }

            public IDbCommand CreateCommand()
            {
                return new Com(_con.CreateCommand());
            }

            public string Database
            {
                get { return _con.Database; }
            }

            public void Open()
            {
                _con.Open();
            }

            public ConnectionState State
            {
                get { return _con.State; }
            }

            public void Dispose()
            {
                _con.Dispose();
            }
        }

        class Com : IDbCommand
        {
            private readonly IDbCommand _com;

            public Com(IDbCommand com)
            {
                _com = com;
            }

            public void Cancel()
            {
                _com.Cancel();
            }

            public string CommandText
            {
                get { return _com.CommandText; }
                set { _com.CommandText = value; }
            }

            public int CommandTimeout
            {
                get { return _com.CommandTimeout; }
                set { _com.CommandTimeout = value; }
            }

            public CommandType CommandType
            {
                get { return _com.CommandType; }
                set { _com.CommandType = value; }
            }

            public IDbConnection Connection
            {
                get { return _com.Connection; }
                set { _com.Connection = value; }
            }

            public IDbDataParameter CreateParameter()
            {
                return _com.CreateParameter();
            }

            public int ExecuteNonQuery()
            {
                return _com.ExecuteNonQuery();
            }

            public IDataReader ExecuteReader(CommandBehavior behavior)
            {
                return _com.ExecuteReader(behavior);
            }

            public IDataReader ExecuteReader()
            {
                return _com.ExecuteReader();
            }

            public object ExecuteScalar()
            {
                return _com.ExecuteScalar();
            }

            public IDataParameterCollection Parameters
            {
                get { return _com.Parameters; }
            }

            public void Prepare()
            {
                _com.Prepare();
            }

            public IDbTransaction Transaction
            {
                get { return _com.Transaction; }
                set { _com.Transaction = value; }
            }

            public UpdateRowSource UpdatedRowSource
            {
                get { return _com.UpdatedRowSource; }
                set { _com.UpdatedRowSource = value; }
            }

            public void Dispose()
            {
                _com.Dispose();
            }
        }

#endif
    }
}
