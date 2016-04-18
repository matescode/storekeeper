using System.Collections.Generic;

using CommonBase.Application.Messages;
using CommonBase.UI.Windows;

namespace CommonBase.UI.MessageDialogs
{
    internal class MessageDialog : IMessageDialog
    {
        private string _title;
        private QuestionResult _result = QuestionResult.Cancel;

        public MessageDialog(string title)
        {
            _title = title;
        }

        #region IMessageDialog Members

        public string Title
        {
            set
            {
                _title = value;
            }
        }

        public void Show(MessageType type, string text)
        {
            ShowInternal(type, text, null);
        }

        public void Show(MessageType type, string text, IEnumerable<string> detail)
        {
            ShowInternal(type, text, detail);
        }

        public QuestionResult Question(string text, bool withCancel = false)
        {
            ShowInternal(MessageType.Question, text, null, withCancel);
            return _result;
        }

        #endregion

        #region Internals and Helpers

        private void ShowInternal(MessageType messageType, string text, IEnumerable<string> detail, bool withCancel = false)
        {
            MessageDialogWindow window = new MessageDialogWindow();
            window.Caption = _title;
            window.MessageType = messageType;
            window.Text = text;
            window.Detail = detail;
            window.QuestionWithCancel = withCancel;
            window.ShowMessageWindow();
            _result = window.Result;
        }

        #endregion
    }
}