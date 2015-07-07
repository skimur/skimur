using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Handling
{
    /// <summary>
    /// Marker interface that makes it easier to discover handlers via reflection.
    /// </summary>
    public interface IEventHandler { }

    public interface IEventHandler<in T> : IEventHandler
        where T : IEvent
    {
        void Handle(T @event);
    }
}
