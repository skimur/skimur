using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Skimur.Data;
using Skimur.Data.Impl;
using Skimur.Email;
using Skimur.Sms;

namespace Skimur
{
    public static class SkimurContext
    {
        private static IServiceProvider _serviceProvider;
        private static object _lock = new object();

        static SkimurContext()
        {

        }

        public static void Initialize(params IRegistrar[] registrars)
        {
            lock (_lock)
            {
                _serviceProvider = BuildServiceProvider(registrars);
            }
        }

        public static IServiceProvider BuildServiceProvider(params IRegistrar[] registrars)
        {
            var collection = new ServiceCollection();

            collection.AddSingleton<IServiceCollection>(provider => collection);

            // all the default services
            collection.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();
            collection.AddSingleton<IDbConnectionProvider, SqlConnectionProvider>();
            collection.AddSingleton<IEmailSender, EmailSender>();
            collection.AddSingleton<ISmsSender, NoSmsSender>();

            foreach (var registrar in registrars.OrderBy(x => x.Order))
                registrar.Register(collection);

            return collection.BuildServiceProvider();
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
