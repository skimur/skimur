using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Handling
{
    public interface IEventRegistrar
    {
        void RegisterEvent<T, TEventHandler>()
            where T : class, IEvent
            where TEventHandler : class, IEventHandler<T>;
    }
}
