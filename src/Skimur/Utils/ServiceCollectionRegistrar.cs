using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Utils
{
    public class ServiceCollectionRegistrar : IRegistrar
    {
        IServiceCollection _serviceCollection;

        public ServiceCollectionRegistrar(IServiceCollection serviceCollection, int order)
        {
            _serviceCollection = serviceCollection;
            Order = order;
        }

        public int Order { get; }

        public void Register(IServiceCollection serviceCollection)
        {
            foreach (var service in _serviceCollection)
            {
                serviceCollection.Add(service);
            }
        }
    }
}
