using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;
using Infrastructure.Cassandra.Migrations;
using SimpleInjector;
using Skimur;

namespace Infrastructure.Cassandra
{
    public class Registrar : IRegistrar
    {
        public void Register(Container container)
        {
            container.RegisterSingleton<ICassandraConnectionStringProvider, CassandraConnectionStringProvider>();
            container.RegisterSingleton(() =>
            {
                var connectionProvider = container.GetInstance<ICassandraConnectionStringProvider>();

                if(!connectionProvider.HasConnectionString)
                    throw new Exception("No connection string configured for cassandra.");

                return Cluster.Builder()
                    .AddContactPoint(connectionProvider.ConnectionString)
                    .WithDefaultKeyspace("skimur")
                    .Build();
            });
            container.RegisterSingleton(() =>
            {
                var cluster = container.GetInstance<Cluster>();
                return cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();
            });
            container.RegisterSingleton<IMigrationEngine, MigrationEngine>();
            container.RegisterSingleton<IMigrationResourceFinder, MigrationResourceFinder>();
        }

        public int Order { get { return 0; } }
    }
}
