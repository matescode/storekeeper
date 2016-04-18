using CommonBase.Exceptions;
using System.Diagnostics;

namespace CommonBase
{
    public abstract class SingletonInstance<T> where T : class
    {
        protected SingletonInstance()
        {
            if (Instance != null)
            {
                throw new SingletonAlreadyInitializedException(GetType());
            }
            Instance = this as T;
        }

        #region Properties

        public static bool IsValid
        {
            get
            {
                return Instance != null;
            }
        }

        protected static T Instance
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        protected static void CloseInstance()
        {
            if (Instance == null)
            {
                Debug.Assert(false, "Instance is already closed.");
            }
            Instance = null;
        }

        #endregion Methods
    }
}