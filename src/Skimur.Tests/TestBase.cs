using NUnit.Framework;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skimur.Tests
{
    public abstract class TestBase
    {
        protected Container _container;

        protected virtual List<IRegistrar> GetRegistrars()
        {
            return new List<IRegistrar>();
        }

        [SetUp]
        protected virtual void Setup()
        {
            if (_container != null) throw new Exception("The context was already initialized!");
            _container = new Container(new ContainerOptions { AllowOverridingRegistrations = true });
            foreach (var registrar in GetRegistrars().OrderBy(x => x.Order))
                registrar.Register(_container);
        }
    }
}
