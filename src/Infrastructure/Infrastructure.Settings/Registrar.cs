using System.Diagnostics;
using SimpleInjector.Extensions;
using Skimur;

namespace Infrastructure.Settings
{
    public class Registrar : IRegistrar
    {
        public void Register(SimpleInjector.Container container)
        {
            var o = new object();
            container.RegisterSingleton(typeof (ISettingsProvider<>), typeof (JsonFileSettingsProvider<>));
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
