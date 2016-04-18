namespace CommonBase.Arguments
{
    public abstract class ValueArgument<T> : Argument
    {
        private T _value;

        public T Value
        {
            get
            {
                return _value;
            }
            internal set
            {
                _value = value;
            }
        }

        protected abstract T Default { get; }
    }
}