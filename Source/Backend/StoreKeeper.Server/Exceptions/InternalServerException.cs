using System;
using System.Runtime.Serialization;

namespace StoreKeeper.Server.Exceptions
{
    public class InternalServerException : Exception
    {
        protected InternalServerException(Type type, int eventId)
        {
            Type = type;
            EventId = eventId;
        }

        protected InternalServerException(Type type, int eventId, string message)
            : base(message)
        {
            Type = type;
            EventId = eventId;
        }

        protected InternalServerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Type Type { get; private set; }

        public int EventId { get; private set; }
    }
}