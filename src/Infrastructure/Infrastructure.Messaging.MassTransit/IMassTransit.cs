using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magnum.Cryptography.PKI;

namespace Infrastructure.Messaging.MassTransit
{
    public interface IMassTransit : IDisposable
    {
        void Publish<T>(T command) where T : class;
    }
}
