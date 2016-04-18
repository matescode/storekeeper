using System;

namespace CommonBase.Log
{
    public interface ILog
    {
        void Close();

        #region Info

        void Info(Type type, int id, string message, params object[] arguments);

        void Info(Type type, string message, params object[] arguments);

        #endregion

        #region Warning

        void Warning(Type type, int id, string message, params object[] arguments);

        void Warning(Type type, string message, params object[] arguments);

        #endregion

        #region Error

        void Error(Type type, int id, string message, params object[] arguments);

        void Error(Type type, string message, params object[] arguments);

        void Error(Type type, Exception ex);

        void Error(Type type, int id, Exception ex);

        #endregion

        #region Critical

        void Critical(Type type, int id, string message, params object[] arguments);

        void Critical(Type type, string message, params object[] arguments);

        void Critical(Type type, Exception ex);

        void Critical(Type type, int id, Exception ex);

        #endregion

        #region Debug

        void Debug(Type type, int id, string message, params object[] arguments);

        void Debug(Type type, string message, params object[] arguments);

        #endregion
    }
}