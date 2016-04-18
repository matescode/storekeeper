using System;
using System.Diagnostics;

namespace CommonBase.Log.Logs
{
    public class SystemEventLog : LogBase
    {
        private EventLog _eventLog;
        private readonly string _source;
        private readonly string _log;

        public SystemEventLog(string source, string log)
            : base(LogMode.SystemLog | LogMode.Console)
        {
            _source = source;
            _log = log;
            InitLog();
        }

        #region Overrides

        public override void LogRecordInternal(LogLevel level, Type type, int id, string message)
        {
            id = id < 0 ? 0 : id;
            _eventLog.WriteEntry(message, GetEntryType(level), id);
        }

        protected override bool CanBeLogged(LogLevel level)
        {
            return level != LogLevel.Debug;
        }

        protected override void OnClosing()
        {
            _eventLog.Close();
        }

        #endregion

        #region Internals and Helpers

        private void InitLog()
        {
            if (!EventLog.SourceExists(_source))
            {
                EventLog.CreateEventSource(_source, _log);
            }

            _eventLog = new EventLog();
            _eventLog.Source = _source;
            _eventLog.Log = _log;
        }

        private EventLogEntryType GetEntryType(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                    return EventLogEntryType.Error;

                case LogLevel.Warning:
                    return EventLogEntryType.Warning;

                case LogLevel.Info:
                    return EventLogEntryType.Information;
            }

            return EventLogEntryType.Error;
        }

        #endregion
    }
}