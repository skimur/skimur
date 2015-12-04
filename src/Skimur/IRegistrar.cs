using Microsoft.Extensions.DependencyInjection;

namespace Skimur
{
    public interface IRegistrar
    {
        void Register(IServiceCollection serviceCollection);

        int Order { get; }
    }
}
