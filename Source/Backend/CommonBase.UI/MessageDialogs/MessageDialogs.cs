using System;
using System.Collections.Generic;
using System.Linq;

using CommonBase.Application.Messages;

namespace CommonBase.UI.MessageDialogs
{
    internal class MessageDialogs : IMessageDialogs
    {
        private string _title;

        public MessageDialogs(string title)
        {
            _title = title;
            ShowMessages = true;
        }

        #region Properties

        private IMessageStack Stack
        {
            get
            {
                return UIApplication.MessageStack;
            }
        }

        #endregion Properties

        #region IMessageDialogs Members

        public bool ShowMessages { get; set; }

        public void Error(Exception ex)
        {
            Stack.AddError(ex);
            ShowInternal();
        }

        public void Error(string text)
        {
            Stack.AddError(text);
            ShowInternal();
        }

        public void Error(string text, params object[] arguments)
        {
            Stack.AddError(text, arguments);
            ShowInternal();
        }

        public void Warning(string text)
        {
            Stack.AddWarning(text);
            ShowInternal();
        }

        public void Warning(string text, params object[] arguments)
        {
            Stack.AddWarning(text, arguments);
            ShowInternal();
        }

        public void Info(string text)
        {
            Stack.AddInfo(text);
            ShowInternal();
        }

        public void Info(string text, params object[] arguments)
        {
            Stack.AddInfo(text, arguments);
            ShowInternal();
        }

        public QuestionResult Question(string text)
        {
            return ShowInternalQuestion(text, null);
        }

        public QuestionResult Question(string text, params object[] arguments)
        {
            return ShowInternalQuestion(text, arguments);
        }

        #endregion

        #region Internals and Helpers

        private void ShowInternal()
        {
            if (!ShowMessages)
            {
                Stack.Clear();
            }

            if (Stack.Count == 0)
            {
                return;
            }

            IMessageDialog dialog = new MessageDialog(_title);
            IMessage message = Stack[0];
            IEnumerable<string> detail = Stack.Skip(1).Select(m => m.Text);
            if (detail.Count() > 0)
            {
                dialog.Show(message.MessageType, message.Text, detail);
            }
            else
            {
                dialog.Show(message.MessageType, message.Text);
            }

            Stack.Clear();
        }

        private QuestionResult ShowInternalQuestion(string text, params object[] arguments)
        {
            IMessageDialog dialog = new MessageDialog(_title);
            string message = (arguments != null) ? string.Format(text, arguments) : text;
            return dialog.Question(message);
        }

        #endregion
    }
}