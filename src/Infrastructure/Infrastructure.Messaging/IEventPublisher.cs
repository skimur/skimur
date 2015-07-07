using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    /// <summary>
    /// Defines that the object exposes events that are meant to be published.
    /// </summary>
    public interface IEventPublisher
    {
        IEnumerable<IEvent> Events { get; }
    }
}
