using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public interface ICommandBus
    {
        void Send<T>(T command) where T : class, ICommand;
        void Send<T>(IEnumerable<T> commands) where T : class, ICommand;
        TResponse Send<TRequest, TResponse>(TRequest command)
            where TRequest : class, ICommandReturns<TResponse>
            where TResponse : class;
    }
}
