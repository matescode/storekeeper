namespace CommonBase.Arguments
{
    public abstract class SwitchArgument : Argument
    {
        private bool _isSet;

        public bool IsSet
        {
            get
            {
                return _isSet;
            }
            internal set
            {
                _isSet = value;
            }
        }
    }
}