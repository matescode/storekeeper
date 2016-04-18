using System;

namespace CommonBase.Utils
{
    public class EasyDispose : IDisposable
    {
        private bool _disposed;

        #region Properties

        protected bool IsDisposed
        {
            get { return _disposed; }
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DisposeAction();
                    _disposed = true;
                }
            }
        }

        protected virtual void DisposeAction()
        {
        }
    }
}