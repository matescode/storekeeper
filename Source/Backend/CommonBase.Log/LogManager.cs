using System;
using System.Collections.Concurrent;

namespace CommonBase.Log
{
    public class LogManager
    {
        private static LogManager _instance = null;

        private ConcurrentDictionary<Type, ILogger> _loggers;

        private LogManager()
        {
            _loggers = new ConcurrentDictionary<Type,ILogger>();
        }

        public static ILogger GetLogger(Type type)
        {
            return Instance._loggers.GetOrAdd(type, t => new Logger(t));
        }

        private static LogManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LogManager();
                }
                return _instance;
            }
        }
    }
}
