using System;

using CommonBase.Log;

namespace StoreKeeper.App.Log
{
    public class LogEntry
    {
        public LogEntry(LogLevel level, Type type, int id, string message)
        {
            Level = level;
            Type = type;
            Id = id;
            Message = message;
            StampTime = DateTime.Now;
        }

        public LogLevel Level { get; private set; }

        public Type Type { get; set; }

        public int Id { get; set; }

        public string Message { get; set; }

        public DateTime StampTime { get; private set; }
    }
}