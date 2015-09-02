using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;
using Skimur;

namespace Infrastructure.Logging
{
    public class Registrar : IRegistrar
    {
        public void Register(Container container)
        {
            container.RegisterSingleton(typeof(ILogger<>), typeof(Logger<>));
        }

        public int Order { get { return 0; } }
    }
}
