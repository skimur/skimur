using System;
using NLog;

namespace Infrastructure.Logging
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
                    
        public void Error(object message, Exception ex = null)
        {
            if (ex == null)
                _log.Error(message);
            else
                _log.Error(ex);
        }

        public void Debug(object message, Exception ex = null)
        {
            if (ex == null)
                _log.Debug(message);
            else
                _log.Debug(ex);
        }

        public void Info(object message, Exception ex = null)
        {
            if (ex == null)
                _log.Info(message);
            else
                _log.Info(ex);
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
