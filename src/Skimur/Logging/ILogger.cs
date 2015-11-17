using System;

namespace Skimur.Logging
{
    public interface ILogger
    {
        void Error(string message, Exception ex = null);

        void Debug(string message, Exception ex = null);

        void Info(string message, Exception ex = null);
    }

    public interface ILogger<T> : ILogger
    {
        
    }
}
