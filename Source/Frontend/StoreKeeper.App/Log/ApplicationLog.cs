using System;
using System.Collections.Generic;
using CommonBase.Log;

namespace StoreKeeper.App.Log
{
    internal class ApplicationLog : LogBase, ILogBrowser
    {
        private readonly List<LogEntry> _entries;

        public ApplicationLog()
            : base(LogMode.Application)
        {
            _entries = new List<LogEntry>();
        }

        #region Overrides

        public override void LogRecordInternal(LogLevel level, Type type, int id, string message)
        {
            if (CanBeLogged(level))
            {
                _entries.Add(CreateLogEntry(level, type, id, message));
            }
        }

        #endregion

        #region ILogBrowser Implementation

        public IEnumerable<LogEntry> LogEntries
        {
            get { return _entries; }
        }

        public void Clear()
        {
            _entries.Clear();
        }

        #endregion

        #region Internals and Helpers

        protected override bool CanBeLogged(LogLevel level)
        {
#if DEBUG
            return true;
#else
            return level != LogLevel.Debug;
#endif
        }

        private LogEntry CreateLogEntry(LogLevel level, Type type, int id, string message)
        {
            return new LogEntry(level, type, id, message);
        }

        #endregion
    }
}