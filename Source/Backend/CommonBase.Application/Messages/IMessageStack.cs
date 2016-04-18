using System;
using System.Collections.Generic;

namespace CommonBase.Application.Messages
{
    public interface IMessageStack : IEnumerable<IMessage>
    {
        bool LogMessages { get; set; }

        IMessage this[int index] { get; }

        int Count { get; }

        void AddError(Exception ex);

        void AddError(string message);

        void AddError(string message, params object[] arguments);

        void AddWarning(string message);

        void AddWarning(string message, params object[] arguments);

        void AddInfo(string message);

        void AddInfo(string message, params object[] arguments);

        void Add(MessageType level, string message, params object[] arguments);

        void Clear();
    }
}