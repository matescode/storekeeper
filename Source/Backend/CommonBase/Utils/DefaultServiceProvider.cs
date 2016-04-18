using System;
using System.Diagnostics;

namespace CommonBase.Utils
{
    public static class DefaultServiceProvider
    {
        public static T GetDefaultProvider<T>(Type serviceProviderType) where T : class
        {
            Debug.Assert(false, string.Format("Service of type {0} is not available on type {1}.", typeof(T).FullName, serviceProviderType.FullName));
            return null;
        }
    }
}