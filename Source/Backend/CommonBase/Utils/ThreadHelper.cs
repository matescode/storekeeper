using System;
using System.Threading;

namespace CommonBase.Utils
{
    public static class ThreadHelper
    {
        public static T EnsureInitialized<T>(ref T currentValue, Func<T> factory, object syncRoot) where T : class
        {
            if (currentValue == null)
            {
                lock (syncRoot)
                {
                    if (currentValue == null)
                    {
                        T tmpValue = factory();
                        Thread.MemoryBarrier();
                        currentValue = tmpValue;
                    }
                }
            }
            return currentValue;
        }
    }
}