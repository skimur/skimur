using System;
using NLog;

namespace Skimur.Logging
{
    public class Logger<T> : Logger, ILogger<T>
    {
        public Logger() :base(typeof(T)) { }
    }

    public class Logger : ILogger
    {
        private readonly NLog.Logger _log;

        public Logger(string name)
        {
            _log = LogManager.GetLogger(name);
        }

        public Logger(Type type) : this(type.Name) { }
                    
        public void Error(string message, Exception ex = null)
        {
            if (ex == null)
                _log.Error(message);
            else
                _log.Error(ex, message);
        }

        public void Debug(string message, Exception ex = null)
        {
            if (ex == null)
                _log.Debug(message);
            else
                _log.Debug(ex, message);
        }

        public void Info(string message, Exception ex = null)
        {
            if (ex == null)
                _log.Info(message);
            else
                _log.Info(ex, message);
        }

        public static ILogger<T> For<T>()
        {
            return new Logger<T>();
        }

        public static ILogger For(Type type)
        {
            return new Logger(type);
        }
    }
}
