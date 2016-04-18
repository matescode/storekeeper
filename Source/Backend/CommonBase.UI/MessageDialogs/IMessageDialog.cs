using System.Collections.Generic;

using CommonBase.Application.Messages;

namespace CommonBase.UI.MessageDialogs
{
    public interface IMessageDialog
    {
        string Title { set; }

        void Show(MessageType type, string text);

        void Show(MessageType type, string text, IEnumerable<string> detail);

        QuestionResult Question(string text, bool withCancel = false);
    }
}