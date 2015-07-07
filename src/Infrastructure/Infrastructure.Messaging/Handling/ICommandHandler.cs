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
    public interface ICommandHandler { }

    public interface ICommandHandler<in TRequest> : ICommandHandler
        where TRequest : ICommand
    {
        void Handle(TRequest command);
    }

    public interface ICommandHandlerResponse<in TRequest, out TResponse> : ICommandHandler
        where TRequest : ICommandReturns<TResponse>
    {
        TResponse Handle(TRequest command);
    }
}
