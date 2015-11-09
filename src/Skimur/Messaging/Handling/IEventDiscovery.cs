namespace Skimur.Messaging.Handling
{
    public interface IEventDiscovery
    {
        void Register(IEventRegistrar registrar);
    }
}
