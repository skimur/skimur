using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Logging
{
    public interface ILogger
    {
        void Error(object message, Exception ex = null);

        void Debug(object message, Exception ex = null);

        void Info(object message, Exception ex = null);
    }

    public interface ILogger<T> : ILogger
    {
        
    }
}
