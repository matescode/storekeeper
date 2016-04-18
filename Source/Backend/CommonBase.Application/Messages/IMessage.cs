namespace CommonBase.Application.Messages
{
    public interface IMessage
    {
        MessageType MessageType
        {
            get;
        }

        string Text
        {
            get;
        }
    }
}