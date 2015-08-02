using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Handling
{
    public interface IEventRegistrar
    {
        void RegisterEvent<T>() where T : class, IEvent;
    }
}
