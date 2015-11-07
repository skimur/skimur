using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.SessionState;
using Cassandra;
using Infrastructure.Caching;
using ServiceStack;
using ServiceStack.RabbitMq;
using ServiceStack.Redis;
using SimpleInjector;
using Skimur.Data;
using Skimur.Email;
using Skimur.Logging;
using Skimur.Messaging;
using Skimur.Messaging.Handling;
using Skimur.Messaging.RabbitMQ;
using Skimur.Settings;
using Cache = System.Web.Caching.Cache;

namespace Skimur
{
    public static class SkimurContext
    {
        private static Container _container;
        private static object _lock = new object();

        static SkimurContext()
        {
            LicenseUtils.RegisterLicense("2283-e1JlZjoyMjgzLE5hbWU6TWVkWENoYW5nZSxUeXBlOkluZGllLEhhc2g6TU" +
                "FyaTVzNGdQcEdlc0pqd1ZIUXVlL0lacDBZcCt3TkFLY0UyMTlJblBuMzRLNWFRb" +
                "HBYN204aGkrQXlRYzUvZnNVUlZzWXd4NjR0OFlXZEpjNUNYRTdnMjBLR0ZjQmhG" +
                "dTFNMHZVazJqcHdQb1RrbStDaHNPRm11Qm50TnZzOTkwcHAzRkxtTC9idThMekN" +
                "lTVRndFBORzBuREZ0WGJUdzdRMi80K09lQ2tZPSxFeHBpcnk6MjAxNi0wMi0xOX" +
                "0=");
        }

        public static event Action<Container> ContainerInitialized = delegate { };

        public static void Initialize(params IRegistrar[] registrars)
        {
            lock (_lock)
            {
                if (_container != null) throw new Exception("The context was already initialized!");
                _container = new Container();
                _container.Options.AllowOverridingRegistrations = true;

                // all the default services
                _container.RegisterSingleton<IMapper, Mapper>();
                _container.RegisterSingleton<IConnectionStringProvider, ConnectionStringProvider>();
                _container.RegisterSingleton<IDbConnectionProvider, SqlConnectionProvider>();
                _container.RegisterSingleton<IEmailSender, EmailSender>();
                _container.RegisterSingleton<IPathResolver, PathResolver>();

                _container.RegisterSingleton<ICache, RedisCache>();
                _container.RegisterSingleton<IRedisClientsManager>(() =>
                {
                    var readWrite = System.Configuration.ConfigurationManager.AppSettings["RedisReadWrite"];
                    var read = System.Configuration.ConfigurationManager.AppSettings["RedisRead"];
                    return new PooledRedisClientManager(readWrite.Split(';'), read.Split(';'));
                });

                _container.RegisterSingleton<Cassandra.ICassandraConnectionStringProvider, Cassandra.CassandraConnectionStringProvider>();
                _container.RegisterSingleton(() =>
                {
                    var connectionProvider = _container.GetInstance<Cassandra.ICassandraConnectionStringProvider>();

                    if (!connectionProvider.HasConnectionString)
                        throw new Exception("No connection string configured for cassandra.");

                    return Cluster.Builder()
                        .AddContactPoint(connectionProvider.ConnectionString)
                        .WithDefaultKeyspace("skimur")
                        .Build();
                });
                _container.RegisterSingleton(() =>
                {
                    var cluster = _container.GetInstance<Cluster>();
                    return cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();
                });
                _container.RegisterSingleton<Cassandra.Migrations.IMigrationEngine, Cassandra.Migrations.MigrationEngine>();
                _container.RegisterSingleton<Cassandra.Migrations.IMigrationResourceFinder, Cassandra.Migrations.MigrationResourceFinder>();

                _container.RegisterSingleton(typeof(ILogger<>), typeof(Logger<>));

                _container.RegisterSingleton<IEventDiscovery, EventDiscovery>();
                _container.RegisterSingleton<ICommandDiscovery, CommandDiscovery>();

                _container.RegisterSingleton<Postgres.Migrations.IMigrationEngine, Postgres.Migrations.MigrationEngine>();
                _container.RegisterSingleton<Postgres.Migrations.IMigrationResourceFinder, Postgres.Migrations.MigrationResourceFinder>();

                _container.RegisterSingleton(typeof(ISettingsProvider<>), typeof(JsonFileSettingsProvider<>));

                _container.RegisterSingleton(() =>
                {
                    var rabbitMqHost = ConfigurationManager.AppSettings["RabbitMQHost"];
                    if (string.IsNullOrEmpty(rabbitMqHost)) throw new Exception("You must provide a 'RabbitMQHost' app setting.");

                    return new RabbitMqServer(rabbitMqHost)
                    {
                        ErrorHandler = exception =>
                        {
                            Logging.Logger.For<RabbitMqServer>().Error("There was an error processing a message.", exception);
                        }
                    };
                });
                _container.RegisterSingleton<ICommandBus, CommandBus>();
                _container.RegisterSingleton<IEventBus, EventBus>();
                _container.RegisterSingleton<IBusLifetime, BusLifetime>();

                foreach (var registrar in registrars.OrderBy(x => x.Order))
                    registrar.Register(_container);
                ContainerInitialized(_container);
            }
        }

        public static T Resolve<T>() where T : class
        {
            EnsureInitialized();
            return _container.GetInstance<T>();
        }

        private static void EnsureInitialized()
        {
            if (_container == null) throw new Exception("The YenContext has not been initialized.");
        }
    }
}
