using System;
using System.Runtime.Serialization;

namespace CommonBase.Exceptions
{
    [Serializable]
    public class CommonException : Exception
    {
        protected CommonException(Type type, int eventId)
            : base()
        {
            Type = type;
            EventId = eventId;
            Log();
        }

        protected CommonException(Type type, int eventId, string message)
            : base(message)
        {
            Type = type;
            EventId = eventId;
            Log();
        }

        protected CommonException(Type type, int eventId, string message, params object[] arguments)
            : base(string.Format(message, arguments))
        {
            Type = type;
            EventId = eventId;
            Log();
        }

        protected CommonException(Type type, int eventId, Exception innerException)
            : base(string.Empty, innerException)
        {
            Type = type;
            EventId = eventId;
            Log();
        }

        protected CommonException(Type type, int eventId, Exception innerException, string message)
            : base(message, innerException)
        {
            Type = type;
            EventId = eventId;
            Log();
        }

        protected CommonException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #region Properties

        protected Type Type
        {
            get;
            private set;
        }

        protected int EventId
        {
            get;
            private set;
        }

        #endregion Properties

        #region Internals and Helpers

        private void Log()
        {
            LogProvider.Instance.Logger.LogException(Type, EventId, Message, InnerException);
        }

        #endregion Internals and Helpers
    }
}