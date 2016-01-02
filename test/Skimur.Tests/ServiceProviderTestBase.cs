using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Skimur.Tests
{
    public abstract class ServiceProviderTestBase
    {
        // ReSharper disable InconsistentNaming
        protected IServiceProvider _serviceProvider;
        // ReSharper restore InconsistentNaming

        protected ServiceProviderTestBase(params IRegistrar[] registrars)
        {
            var tmp = registrars.ToList();
            tmp.Add(new InternalRegistrar());
            _serviceProvider = SkimurContext.BuildServiceProvider(tmp.ToArray());
        }

        private class InternalRegistrar : IRegistrar
        {
            public int Order => 0;

            public void Register(IServiceCollection serviceCollection)
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                builder.AddEnvironmentVariables();
                serviceCollection.AddInstance<IConfiguration>(builder.Build());
            }
        }
    }
}
