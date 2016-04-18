using System;

namespace CommonBase.UI.MessageDialogs
{
    public interface IMessageDialogs
    {
        bool ShowMessages { get; set; }

        void Error(Exception ex);

        void Error(string text);

        void Error(string text, params object[] arguments);

        void Warning(string text);

        void Warning(string text, params object[] arguments);

        void Info(string text);

        void Info(string text, params object[] arguments);

        QuestionResult Question(string text);

        QuestionResult Question(string text, params object[] arguments);
    }
}