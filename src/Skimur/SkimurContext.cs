using System;
using System.Configuration;
using System.Linq;
using Cassandra;
using Infrastructure.Caching;
using ServiceStack;
using ServiceStack.RabbitMq;
using ServiceStack.Redis;
using Skimur.Data;
using Skimur.Email;
using Skimur.Embed;
using Skimur.Logging;
using Skimur.Messaging;
using Skimur.Messaging.Handling;
using Skimur.Messaging.RabbitMQ;
using Skimur.Settings;
using Cache = System.Web.Caching.Cache;
using Microsoft.Extensions.DependencyInjection;
using Skimur.Cassandra;
using System.Diagnostics;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace Skimur
{
    public static class SkimurContext
    {
        private static IServiceProvider _serviceProvider;
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

        public static event Action<IServiceProvider> ContainerInitialized = delegate { };

        public static void Initialize(params IRegistrar[] registrars)
        {
            lock (_lock)
            {
                var collection = new ServiceCollection();

                collection.AddSingleton<IServiceCollection>(provider => collection);

                // all the default services
                collection.AddSingleton<IMapper, Mapper>();
                collection.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();
                collection.AddSingleton<IDbConnectionProvider, SqlConnectionProvider>();
                collection.AddSingleton<IEmailSender, EmailSender>();
                collection.AddSingleton<IPathResolver, PathResolver>();

                collection.AddSingleton<ICache, RedisCache>();
                collection.AddSingleton<IRedisClientsManager>(provider =>{
                    var configuration = provider.GetService<IConfiguration>();
                    var readWrite = configuration.Get<string>("Data:RedisReadWrite");
                    var read = configuration.Get<string>("Data:RedisRead");
                    return new PooledRedisClientManager(readWrite.Split(';'), read.Split(';'));
                });

                collection.AddSingleton<ICassandraConnectionStringProvider, CassandraConnectionStringProvider>();
                collection.AddSingleton(provider =>
                {
                    var connectionProvider = provider.GetService<ICassandraConnectionStringProvider>();

                    if (!connectionProvider.HasConnectionString)
                        throw new Exception("No connection string configured for cassandra.");

                    return Cluster.Builder()
                        .AddContactPoint(connectionProvider.ConnectionString)
                        .WithDefaultKeyspace("skimur")
                        .Build();
                });
                collection.AddSingleton(provider =>
                {
                    var cluster = provider.GetService<Cluster>();
                    return cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();
                });
                collection.AddSingleton<Cassandra.Migrations.IMigrationEngine, Cassandra.Migrations.MigrationEngine>();
                collection.AddSingleton<Cassandra.Migrations.IMigrationResourceFinder, Cassandra.Migrations.MigrationResourceFinder>();

                collection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

                collection.AddSingleton<IEventDiscovery, EventDiscovery>();
                collection.AddSingleton<ICommandDiscovery, CommandDiscovery>();

                collection.AddSingleton<Postgres.Migrations.IMigrationEngine, Postgres.Migrations.MigrationEngine>();
                collection.AddSingleton<Postgres.Migrations.IMigrationResourceFinder, Postgres.Migrations.MigrationResourceFinder>();

                collection.AddSingleton(typeof(ISettingsProvider<>), typeof(JsonFileSettingsProvider<>));

                collection.AddSingleton(provider =>
                {
                    var configuration = provider.GetService<IConfiguration>();
                    var rabbitMqHost = configuration.Get<string>("Data:RabbitMQHost");
                    if (string.IsNullOrEmpty(rabbitMqHost)) throw new Exception("You must provide a 'RabbitMQHost' app setting.");

                    return new RabbitMqServer(rabbitMqHost)
                    {
                        ErrorHandler = exception =>
                        {
                            Logging.Logger.For<RabbitMqServer>().Error("There was an error processing a message.", exception);
                        }
                    };
                });
                collection.AddSingleton<ICommandBus, CommandBus>();
                collection.AddSingleton<IEventBus, EventBus>();
                collection.AddSingleton<IBusLifetime, BusLifetime>();

                collection.AddSingleton<IEmbeddedProvider, ContextualEmbededProvider>();
                
                foreach (var registrar in registrars.OrderBy(x => x.Order))
                    registrar.Register(collection);
                
                _serviceProvider = collection.BuildServiceProvider();
                
                ContainerInitialized(_serviceProvider);
            }
        }

        public static object Resolve(Type type)
        {
            EnsureInitialized();
            if(HttpContext.Current == null)
            {
                return _serviceProvider.GetService(type);
            }
            else
            {
                var scope = HttpContext.Current.Items["Scope"] as IServiceScope;
                if(scope != null)
                {
                    return scope.ServiceProvider.GetService(type);
                }
                else
                {
                    return _serviceProvider.GetService(type);
                }
            }
        }

        public static T Resolve<T>() where T : class
        {
            return Resolve(typeof(T)) as T;
        }

        public static IServiceProvider ServiceProvider
        {
            get
            {
                EnsureInitialized();
                return _serviceProvider;
            }
        }

        private static void EnsureInitialized()
        {
            if (_serviceProvider == null) throw new Exception("The YenContext has not been initialized.");
        }
    }
}
