using Microsoft.Extensions.DependencyInjection;

namespace Skimur.Scraper
{
    public class Registrar : IRegistrar
    {
        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMediaScrapper, MediaScrapper>();
        }

        public int Order { get { return 0; } }
    }
}
