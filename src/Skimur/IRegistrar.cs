using SimpleInjector;

namespace Skimur
{
    public interface IRegistrar
    {
        void Register(Container container);

        int Order { get; }
    }
}
