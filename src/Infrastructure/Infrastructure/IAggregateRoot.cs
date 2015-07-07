using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    /// <summary>
    /// Represents an identifiable entity in the system.
    /// </summary>
    public interface IAggregateRoot
    {
        Guid Id { get; }
    }
}
