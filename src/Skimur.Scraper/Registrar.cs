using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;

namespace Skimur.Scraper
{
    public class Registrar : IRegistrar
    {
        public void Register(Container container)
        {
            container.RegisterSingleton<IMediaScrapper, MediaScrapper>();
        }

        public int Order { get { return 0; } }
    }
}
