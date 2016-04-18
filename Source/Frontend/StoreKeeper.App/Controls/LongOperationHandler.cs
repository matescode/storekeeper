using System;
using System.Threading;
using System.Threading.Tasks;
using CommonBase.UI;
using CommonBase.UI.Localization;
using CommonBase.UI.Windows;
using StoreKeeper.Client;

namespace StoreKeeper.App.Controls
{
    public class LongOperationHandler : ILongOperationHandler
    {
        private readonly TaskScheduler _taskScheduler;
        private readonly Action<ILongOperationResult> _processResultAction;
        private BusyWindow _progressWindow;

        public LongOperationHandler(TaskScheduler scheduler)
        {
            _taskScheduler = scheduler;
        }

        public LongOperationHandler(TaskScheduler scheduler, Action<ILongOperationResult> processResultAction)
            : this(scheduler)
        {
            _processResultAction = processResultAction;
        }
        #region ILongOperationHandler Implementation

        public TaskScheduler TaskScheduler
        {
            get { return _taskScheduler; }
        }

        public void Start(string operationName = null, string firstOperationPart = null)
        {
            ExecuteAsync(() =>
            {
                _progressWindow = new BusyWindow();
                if (!String.IsNullOrWhiteSpace(operationName))
                {
                    string localizedName = operationName.Localize();
                    _progressWindow.WindowTitle = localizedName;
                    _progressWindow.OperationName = localizedName + " ...";
                }
                if (!String.IsNullOrWhiteSpace(firstOperationPart))
                {
                    _progressWindow.OperationName = firstOperationPart.Localize() + " ...";
                }
                _progressWindow.ShowDialog();
            });
        }

        public void Next(string operationPart)
        {
            ExecuteAsync(() =>
            {
                if (_progressWindow != null)
                {
                    _progressWindow.OperationName = operationPart.Localize() + " ...";
                }
            });
        }

        public void End(ILongOperationResult result)
        {
            ExecuteAsync(() =>
            {
                if (_progressWindow != null)
                {
                    _progressWindow.ForceClose();
                }
                _processResultAction(result);
            });
        }

        public void End(Action finalAction = null)
        {
            ExecuteAsync(() =>
            {
                if (_progressWindow != null)
                {
                    _progressWindow.ForceClose();
                }

                if (finalAction != null)
                {
                    finalAction();
                }
            });
        }

        public void OperationFailed(string reason, bool localize = false)
        {
            ExecuteAsync(() =>
            {
                if (_progressWindow != null)
                {
                    UIApplication.MessageDialogs.Error(localize ? reason.Localize() : reason);
                    _progressWindow.ForceClose();
                }
            });
        }

        public void Refresh(ILongOperationResult result)
        {
            ExecuteAsync(() => _processResultAction(result));
        }

        #endregion

        #region Internals and Helpers

        private void ExecuteAsync(Action method)
        {
            Task.Factory.StartNew(method, new CancellationToken(), TaskCreationOptions.None, _taskScheduler);
        }

        #endregion
    }
}