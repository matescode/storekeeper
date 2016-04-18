using System;

namespace CommonBase.Log
{
    internal sealed class Logger : ILogger
    {
        private Type _type;

        public Logger(Type type)
        {
            _type = type;
        }

        #region ILogger Implementation

        #region Info

        public void Info(int id, string message, params object[] arguments)
        {
            LogBase.Instance.Info(_type, id, message, arguments);
        }

        public void Info(string message, params object[] arguments)
        {
            LogBase.Instance.Info(_type, message, arguments);
        }

        #endregion

        #region Warning

        public void Warning(int id, string message, params object[] arguments)
        {
            LogBase.Instance.Warning(_type, id, message, arguments);
        }

        public void Warning(string message, params object[] arguments)
        {
            LogBase.Instance.Warning(_type, message, arguments);
        }

        #endregion

        #region Error

        public void Error(int id, string message, params object[] arguments)
        {
            LogBase.Instance.Error(_type, id, message, arguments);
        }

        public void Error(string message, params object[] arguments)
        {
            LogBase.Instance.Error(_type, message, arguments);
        }

        public void Error(Exception ex)
        {
            LogBase.Instance.Error(_type, ex);
        }

        public void Error(int id, Exception ex)
        {
            LogBase.Instance.Error(_type, id, ex);
        }

        #endregion

        #region Critical

        public void Critical(int id, string message, params object[] arguments)
        {
            LogBase.Instance.Critical(_type, id, message, arguments);
        }

        public void Critical(string message, params object[] arguments)
        {
            LogBase.Instance.Critical(_type, message, arguments);
        }

        public void Critical(Exception ex)
        {
            LogBase.Instance.Critical(_type, ex);
        }

        public void Critical(int id, Exception ex)
        {
            LogBase.Instance.Critical(_type, id, ex);
        }

        #endregion

        #region Debug

        public void Debug(int id, string message, params object[] arguments)
        {
            LogBase.Instance.Debug(_type, id, message, arguments);
        }

        public void Debug(string message, params object[] arguments)
        {
            LogBase.Instance.Debug(_type, message, arguments);
        }

        #endregion

        #endregion
    }
}
