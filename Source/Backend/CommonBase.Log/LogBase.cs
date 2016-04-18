using System;
using System.Text;

namespace CommonBase.Log
{
    public abstract class LogBase : ILog
    {
        private LogMode _logMode;

        protected LogBase(LogMode logMode)
        {
            _logMode = logMode;
            Instance = this;
        }

        #region Properties

        internal static LogBase Instance
        {
            get;
            private set;
        }

        protected LogMode LogMode
        {
            get
            {
                return _logMode;
            }
        }

        #endregion

        #region Methods

        public abstract void LogRecordInternal(LogLevel level, Type type, int id, string message);

        protected abstract bool CanBeLogged(LogLevel level);

        protected internal void LogRecord(LogLevel level, Type type, int id, Exception ex, string message, params object[] arguments)
        {
            if (_logMode == LogMode.None)
            {
                return;
            }

            if (ex == null && string.IsNullOrEmpty(message))
            {
                // nothing to log
                return;
            }

            string recordMessage = FormatMessage(level, type, id, ex, message, arguments);

            if (_logMode.HasFlag(LogMode.Console) && Environment.UserInteractive)
            {
                LogToConsole(level, recordMessage);
            }

            if (CanBeLogged(level))
            {
                LogRecordInternal(level, type, id, recordMessage);
            }
        }

        protected virtual void OnClosing()
        {
        }

        #endregion

        #region Internals and Helpers

        private void LogToConsole(LogLevel level, string message)
        {
            SetColorToLevel(level);
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private void SetColorToLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LogLevel.Critical:
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
        }

        private string FormatMessage(LogLevel level, Type type, int id, Exception ex, string message, params object[] arguments)
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            result.Append(level.ToString());
            result.Append("] @ ");
            result.AppendFormat("{0}-{1}-{2} {3}:{4}:{5}", DateTime.Now.Year.ToString("D4"), DateTime.Now.Month.ToString("D2"), DateTime.Now.Day.ToString("D2"), DateTime.Now.Hour.ToString("D2"), DateTime.Now.Minute.ToString("D2"), DateTime.Now.Second.ToString("D2"));
            result.AppendFormat(" @ {0}", type == null ? "-" : type.Name);
            result.AppendFormat(" : {0} : ", id == -1 ? "-" : id.ToString());

            if (ex != null)
            {
                result.AppendFormat("Exception {0} occured.", ex.GetType());
                if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    result.AppendFormat("\n{0}\n", ex.StackTrace);
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                if (arguments != null)
                {
                    result.AppendFormat(message, arguments);
                }
                else
                {
                    result.Append(message);
                }
            }

            return result.ToString();
        }

        #endregion

        #region ILog Implementation

        public void Close()
        {
            Instance = null;
        }

        #region Info

        public void Info(Type type, int id, string message, params object[] arguments)
        {
            LogRecord(LogLevel.Info, type, id, null, message, arguments);
        }

        public void Info(Type type, string message, params object[] arguments)
        {
            LogRecord(LogLevel.Info, type, -1, null, message, arguments);
        }

        #endregion

        #region Warning

        public void Warning(Type type, int id, string message, params object[] arguments)
        {
            LogRecord(LogLevel.Warning, type, id, null, message, arguments);
        }

        public void Warning(Type type, string message, params object[] arguments)
        {
            LogRecord(LogLevel.Warning, type, -1, null, message, arguments);
        }

        #endregion

        #region Error

        public void Error(Type type, int id, string message, params object[] arguments)
        {
            LogRecord(LogLevel.Error, type, id, null, message, arguments);
        }

        public void Error(Type type, string message, params object[] arguments)
        {
            LogRecord(LogLevel.Error, type, -1, null, message, arguments);
        }

        public void Error(Type type, Exception ex)
        {
            LogRecord(LogLevel.Error, type, -1, ex, null, null);
        }

        public void Error(Type type, int id, Exception ex)
        {
            LogRecord(LogLevel.Error, type, id, ex, null, null);
        }

        #endregion

        #region Critical

        public void Critical(Type type, int id, string message, params object[] arguments)
        {
            LogRecord(LogLevel.Critical, type, id, null, message, arguments);
        }

        public void Critical(Type type, string message, params object[] arguments)
        {
            LogRecord(LogLevel.Critical, type, -1, null, message, arguments);
        }

        public void Critical(Type type, Exception ex)
        {
            LogRecord(LogLevel.Critical, type, -1, ex, null, null);
        }

        public void Critical(Type type, int id, Exception ex)
        {
            LogRecord(LogLevel.Critical, type, id, ex, null, null);
        }

        #endregion

        #region Debug

        public void Debug(Type type, int id, string message, params object[] arguments)
        {
            LogRecord(LogLevel.Debug, type, id, null, message, arguments);
        }

        public void Debug(Type type, string message, params object[] arguments)
        {
            LogRecord(LogLevel.Debug, type, -1, null, message, arguments);
        }

        #endregion

        #endregion
    }
}