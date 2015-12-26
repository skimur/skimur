using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skimur.Tests
{
    public abstract class TestBase
    {
        protected IServiceProvider _serviceProvider;

        protected virtual List<IRegistrar> GetRegistrars()
        {
            return new List<IRegistrar>();
        }

        [SetUp]
        protected virtual void Setup()
        {
            var serviceCollection = new ServiceCollection();
            foreach (var registrar in GetRegistrars().OrderBy(x => x.Order))
                registrar.Register(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
