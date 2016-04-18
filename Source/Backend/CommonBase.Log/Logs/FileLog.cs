using System;

namespace CommonBase.Log.Logs
{
    public class FileLog : LogBase
    {
        public FileLog()
            : base(LogMode.Console | LogMode.File)
        {
        }

        #region Overrides

        public override void LogRecordInternal(LogLevel level, Type type, int id, string message)
        {
        }

        protected override bool CanBeLogged(LogLevel level)
        {
            return true;
        }

        #endregion
    }
}
