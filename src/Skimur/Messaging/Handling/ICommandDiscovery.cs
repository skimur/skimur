namespace Skimur.Messaging.Handling
{
    public interface ICommandDiscovery
    {
        void Register(ICommandRegistrar registrar);
    }
}
