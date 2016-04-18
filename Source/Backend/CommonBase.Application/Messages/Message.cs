namespace CommonBase.Application.Messages
{
    internal class Message : IMessage
    {
        public Message(MessageType level, string text)
        {
            MessageType = level;
            Text = text;
        }

        #region IMessage Implementation

        public MessageType MessageType
        {
            get;
            private set;
        }

        public string Text
        {
            get;
            private set;
        }

        #endregion IMessage Implementation
    }
}