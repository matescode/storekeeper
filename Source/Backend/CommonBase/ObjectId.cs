using System;
using System.Runtime.Serialization;

namespace CommonBase
{
    [DataContract]
    [Serializable]
    public class ObjectId
    {
        public static readonly ObjectId Empty = new ObjectId(Guid.Empty);

        private Guid _id;

        public ObjectId(string id)
        {
            _id = Guid.Parse(id);
        }

        public ObjectId(Guid id)
        {
            _id = id;
        }

        #region Public Methods

        public static ObjectId NewId()
        {
            return new ObjectId(Guid.NewGuid());
        }

        public static ObjectId Parse(string input)
        {
            return new ObjectId(Guid.Parse(input));
        }

        public static bool IsNullOrEmpty(ObjectId id)
        {
            return id == null || id == Empty;
        }

        public static bool TryParse(string input, out ObjectId result)
        {
            Guid guid;
            result = null;
            if (Guid.TryParse(input, out guid))
            {
                result = new ObjectId(guid);
                return true;
            }
            return false;
        }

        public static implicit operator Guid(ObjectId objectId)
        {
            return objectId._id;
        }

        public static implicit operator ObjectId(Guid guid)
        {
            return new ObjectId(guid);
        }

        public Guid ToGuid()
        {
            return _id;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return _id.ToString().ToUpper();
        }

        public override bool Equals(object obj)
        {
            ObjectId other = obj as ObjectId;
            if (other == null)
            {
                return false;
            }

            return _id == other._id;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        #endregion

        #region Operators

        public static bool operator ==(ObjectId a, ObjectId b)
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

        public static bool operator !=(ObjectId a, ObjectId b)
        {
            return !(a == b);
        }

        #endregion
    }
}