using System;

namespace CommonBase.Arguments
{
    public class ArgId
    {
        private const string ArgIdPrefix = "ArgId";

        private Type _type;

        public ArgId(Type type)
        {
            _type = type;
        }

        public override string ToString()
        {
            return string.Format("{0}#{1}", ArgIdPrefix, _type.Name);
        }
    }
}