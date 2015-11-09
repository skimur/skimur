using System;

namespace Skimur.Logging
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
