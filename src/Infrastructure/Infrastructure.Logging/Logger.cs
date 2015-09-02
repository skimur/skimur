using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Infrastructure.Logging
{
    public class Logger<T> : ILogger<T>
    {
        private readonly ILog _log;

        public Logger()
        {
            _log = LogManager.GetLogger(typeof(T));
        }

        public void Error(object message, Exception ex = null)
        {
            if (ex == null)
                _log.Error(message);
            else
                _log.Error(message, ex);
        }

        public void Debug(object message, Exception ex = null)
        {
            if (ex == null)
                _log.Debug(message);
            else
                _log.Debug(message, ex);
        }

        public void Info(object message, Exception ex = null)
        {
            if (ex == null)
                _log.Info(message);
            else
                _log.Info(message, ex);
        }
    }
}
