using System;
using System.Linq;
using SimpleInjector;

namespace Skimur
{
    public static class SkimurContext
    {
        private static Container _container;
        private static object _lock = new object();

        public static void Initialize(params IRegistrar[] registrars)
        {
            lock (_lock)
            {
                if (_container != null) throw new Exception("The context was already initialized!");
                _container = new Container();
                _container.Options.AllowOverridingRegistrations = true;
                foreach (var registrar in registrars.OrderBy(x => x.Order))
                {
                    registrar.Register(_container);
                }
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
