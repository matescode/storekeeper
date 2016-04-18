using System.Collections.Generic;

namespace StoreKeeper.App.Log
{
    public interface ILogBrowser
    {
        IEnumerable<LogEntry> LogEntries { get; }

        void Clear();
    }
}