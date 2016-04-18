namespace CommonBase.Arguments
{
    public abstract class Argument
    {
        public ArgId ArgId
        {
            get;
            internal set;
        }

        public bool IsRegistered
        {
            get
            {
                return ArgId != null;
            }
        }

        protected abstract string ShortName { get; }

        protected abstract string LongName { get; }

        protected abstract bool IsMandatory { get; }

        protected abstract string Help { get; }
    }
}