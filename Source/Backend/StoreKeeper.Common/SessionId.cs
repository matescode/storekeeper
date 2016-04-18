using System;
using System.Runtime.Serialization;

namespace StoreKeeper.Common
{
    [DataContract]
    [Serializable]
    public class SessionId
    {
        [DataMember]
        private string _id;

        public SessionId(string sessionId)
        {
            _id = sessionId;
        }

        #region Overrides

        public override string ToString()
        {
            return _id;
        }

        public override bool Equals(object obj)
        {
            SessionId other = obj as SessionId;
            if (other == null)
            {
                return false; ;
            }

            return _id == other._id;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        #endregion

        #region Operators

        public static bool operator ==(SessionId a, SessionId b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            {
                return true;
            }
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(SessionId a, SessionId b)
        {
            return !(a == b);
        }

        #endregion
    }
}