using System;

namespace CommonBase.Log
{
    public interface ILogger
    {
        #region Info

        void Info(int id, string message, params object[] arguments);

        void Info(string message, params object[] arguments);

        #endregion

        #region Warning

        void Warning(int id, string message, params object[] arguments);

        void Warning(string message, params object[] arguments);

        #endregion

        #region Error

        void Error(int id, string message, params object[] arguments);

        void Error(string message, params object[] arguments);

        void Error(Exception ex);

        void Error(int id, Exception ex);

        #endregion

        #region Critical

        void Critical(int id, string message, params object[] arguments);

        void Critical(string message, params object[] arguments);

        void Critical(Exception ex);

        void Critical(int id, Exception ex);

        #endregion

        #region Debug

        void Debug(int id, string message, params object[] arguments);

        void Debug(string message, params object[] arguments);

        #endregion
    }
}
