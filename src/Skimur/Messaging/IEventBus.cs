using System.Collections.Generic;

namespace Skimur.Messaging
{
    /// <summary>
    /// An event bus that sends serialized object payloads.
    /// </summary>
    public interface IEventBus
    {
        void Publish<T>(T @event) where T : class, IEvent;
        void Publish<T>(IEnumerable<T> events) where T : class, IEvent;
    }
}
