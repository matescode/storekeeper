using System;

namespace CommonBase
{
    public interface IExceptionLogger
    {
        void LogException(Type type, int id, string message, Exception exception);
    }
}
