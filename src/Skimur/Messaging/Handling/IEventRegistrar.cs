namespace Skimur.Messaging.Handling
{
    public interface IEventRegistrar
    {
        void RegisterEvent<T, TEventHandler>()
            where T : class, IEvent
            where TEventHandler : class, IEventHandler<T>;
    }
}
