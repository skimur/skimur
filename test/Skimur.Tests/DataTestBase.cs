using System;
using System.Collections.Generic;
using Cassandra;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using Skimur.Data;
using Microsoft.Extensions.DependencyInjection;
using Skimur.Cassandra;

namespace Skimur.Tests
{
    public abstract class DataTestBase : ServiceProviderTestBase
    {
        // ReSharper disable InconsistentNaming
        protected IDbConnectionProvider _conn;
        // ReSharper restore InconsistentNaming

        protected DataTestBase()
            : base(new Skimur.App.Registrar(),
                new Skimur.App.Handlers.Registrar(),
                new Skimur.Markdown.Registrar(),
                new Skimur.Scraper.Registrar())
        {
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
                using (var cassandraConnection = cassandraCluster.Connect())
                    cassandraConnection.Execute("DROP KEYSPACE skimur");
            
            Cassandra.Migrations.Migrations.Run(_serviceProvider);
            Postgres.Migrations.Migrations.Run(_serviceProvider);
        }
    }
}
