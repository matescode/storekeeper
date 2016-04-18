using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommonBase.Application.Messages
{
    public class MessageStack : IMessageStack
    {
        private Stack<IMessage> _stack;

        public MessageStack()
        {
            _stack = new Stack<IMessage>();
        }

        private void AddInternal(MessageType level, string message, params object[] arguments)
        {
            string messageText;
            if (arguments != null)
            {
                messageText = string.Format(message, arguments);
            }
            else
            {
                messageText = message;
            }

            _stack.Push(new Message(level, messageText));
            if (LogMessages)
            {
                switch (level)
                {
                    case MessageType.Error:
                        ApplicationContext.Log.Error(GetType(), messageText);
                        break;

                    case MessageType.Warning:
                        ApplicationContext.Log.Warning(GetType(), messageText);
                        break;

                    case MessageType.Info:
                        ApplicationContext.Log.Info(GetType(), messageText);
                        break;
                }
            }
        }

        #region IMessageStack Implementation

        public bool LogMessages { get; set; }

        public IMessage this[int index]
        {
            get
            {
                return _stack.ElementAtOrDefault(index);
            }
        }

        public int Count
        {
            get
            {
                return _stack.Count;
            }
        }

        public void Clear()
        {
            _stack.Clear();
        }

        public void AddError(Exception ex)
        {
            Exception lastException = ex;
            do
            {
                AddError(lastException.Message);
                lastException = lastException.InnerException;
            }
            while (lastException != null);
        }

        public void AddError(string message)
        {
            AddInternal(MessageType.Error, message, null);
        }

        public void AddError(string message, params object[] arguments)
        {
            AddInternal(MessageType.Error, message, arguments);
        }

        public void AddWarning(string message)
        {
            AddInternal(MessageType.Warning, message, null);
        }

        public void AddWarning(string message, params object[] arguments)
        {
            AddInternal(MessageType.Warning, message, arguments);
        }

        public void AddInfo(string message)
        {
            AddInternal(MessageType.Info, message, null);
        }

        public void AddInfo(string message, params object[] arguments)
        {
            AddInternal(MessageType.Info, message, arguments);
        }

        public void Add(MessageType level, string message, params object[] arguments)
        {
            AddInternal(level, message, arguments);
        }

        public IEnumerator<IMessage> GetEnumerator()
        {
            return _stack.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _stack.GetEnumerator();
        }

        #endregion IMessageStack Implementation
    }
}