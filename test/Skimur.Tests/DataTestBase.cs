using System;
using System.Collections.Generic;
using System.Threading;
using Cassandra;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using Skimur.Data;
using Microsoft.Extensions.DependencyInjection;
using Skimur.Cassandra;

namespace Skimur.Tests
{
    public abstract class DataTestBase : ServiceProviderTestBase, IDisposable
    {
        // ReSharper disable InconsistentNaming
        protected IDbConnectionProvider _conn;
        // ReSharper restore InconsistentNaming
        private static object _sync = new object();

        protected DataTestBase()
            : base(new Skimur.App.Registrar(),
                new Skimur.App.Handlers.Registrar(),
                new Skimur.Markdown.Registrar(),
                new Skimur.Scraper.Registrar())
        {
            Monitor.Enter(_sync);

            _conn = _serviceProvider.GetService<IDbConnectionProvider>();

            // recreate the entire postgres database
            _conn.Perform(conn =>
            {
                conn.ExecuteSql("drop schema public cascade;create schema public;");
            });
            
            // recreate the entire cassandra keyspace
            using (var cassandraCluster = Cluster.Builder()
                .AddContactPoint(_serviceProvider.GetService<ICassandraConnectionStringProvider>().ConnectionString)
                .Build())
            {
                using (var cassandraConnection = cassandraCluster.Connect())
                {
                    cassandraConnection.DeleteKeyspaceIfExists("skimur");
                    cassandraConnection.CreateKeyspace("skimur");
                }
            }

            Cassandra.Migrations.Migrations.Run(_serviceProvider);
            Postgres.Migrations.Migrations.Run(_serviceProvider);
        }

        public void Dispose()
        {
            Monitor.Exit(_sync);
        }
    }
}
