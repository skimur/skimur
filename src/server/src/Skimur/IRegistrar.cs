using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur
{
    public interface IRegistrar
    {
        void Register(IServiceCollection serviceCollection);

        int Order { get; }
    }
}
