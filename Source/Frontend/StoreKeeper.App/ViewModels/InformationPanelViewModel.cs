using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using CommonBase.Application;
using CommonBase.Resources;
using CommonBase.UI;
using CommonBase.UI.Localization;

using StoreKeeper.Client;

namespace StoreKeeper.App.ViewModels
{
    public class InformationPanelViewModel : ViewModelBase, IClientMessenger
    {
        private readonly IStoreKeeperServiceClient _client;
        private readonly IResourceProvider _resourceProvider;
        private readonly TaskScheduler _taskScheduler;
        private bool _dataUpdated = true;

        public InformationPanelViewModel(TaskScheduler scheduler)
        {
            _taskScheduler = scheduler;
            _client = ApplicationContext.Service<IStoreKeeperServiceClient>();
            _resourceProvider = UIApplication.Service<IResourceProvider>();
        }

        #region Properties

        public string ConnectionStatus
        {
            get
            {
                switch (_client.ConnectionStatus)
                {
                    case Client.ConnectionStatus.Connected:
                        return "ServerConnected".Localize();
                    case Client.ConnectionStatus.Disconnected:
                        return "ServerDisconnected".Localize();
                    case Client.ConnectionStatus.Inactive:
                        return "ServerInvalid".Localize();
                    default:
                        return "ServerDisconnected".Localize();
                }
            }
        }

        public ImageSource ConnectionStatusIconSource
        {
            get
            {
                switch (_client.ConnectionStatus)
                {
                    case Client.ConnectionStatus.Connected:
                        return _resourceProvider.GetResource<ImageSource>("IconOnlineStatus");
                    case Client.ConnectionStatus.Disconnected:
                        return _resourceProvider.GetResource<ImageSource>("IconOfflineStatus");
                    case Client.ConnectionStatus.Inactive:
                        return _resourceProvider.GetResource<ImageSource>("IconInactiveStatus");
                    default:
                        return _resourceProvider.GetResource<ImageSource>("IconOfflineStatus");
                }
            }
        }

        public ImageSource DatabaseStatusIconSource
        {
            get
            {
                switch (_client.DatabaseStatus)
                {
                    case DatabaseStatus.Connected:
                        return _resourceProvider.GetResource<ImageSource>("IconInactiveStatus");
                    case DatabaseStatus.Locked:
                        return _resourceProvider.GetResource<ImageSource>("IconOnlineStatus");
                    case DatabaseStatus.Blocked:
                        return _resourceProvider.GetResource<ImageSource>("IconOfflineStatus");
                    default:
                        return _resourceProvider.GetResource<ImageSource>("IconInactiveStatus");
                }
            }
        }

        public string DatabaseConnectionStatus
        {
            get
            {
                switch (_client.DatabaseStatus)
                {
                    case DatabaseStatus.Connected:
                        return "DBStatus_Connected".Localize();
                    case DatabaseStatus.Locked:
                        return "DBStatus_Locked".Localize();
                    case DatabaseStatus.Blocked:
                        return "DBStatus_Blocked".Localize();
                    default:
                        return "ServerDisconnected".Localize();
                }
            }
        }

        public Visibility CalculationWarningVisibility
        {
            get { return AppContext.Config.NeedsCalculation ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility NewDataInfoVisibility
        {
            get { return !_dataUpdated ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        #region IClientMessenger Implementation

        public void ConnectionClosing()
        {
            RefreshConnectionStatus();
        }

        public void ConnectionRestarted()
        {
            RefreshConnectionStatus();
        }

        public void DatabaseStatusChanged()
        {
            RefreshDatabaseConnectionStatus();
        }

        public void AccountingDataUpdated()
        {
            DataUpdated = false;
        }

        public void StoreKeeperDataUpdated()
        {
            DataUpdated = false;
        }

        public bool CalculationRequested
        {
            set
            {
                AppContext.Config.NeedsCalculation = value;
                NotifyPropertyChanged("CalculationWarningVisibility");
            }
        }

        public bool DataUpdated
        {
            get { return _dataUpdated; }
            set
            {
                _dataUpdated = value;
                NotifyPropertyChanged("NewDataInfoVisibility");
            }
        }

        #endregion

        public void RefreshConnectionStatus()
        {
            ExecuteInMainThreadAsync(() =>
                {
                    NotifyPropertyChanged("ConnectionStatus");
                    NotifyPropertyChanged("ConnectionStatusIconSource");
                    NotifyPropertyChanged("DatabaseStatusIconSource");
                    NotifyPropertyChanged("DatabaseConnectionStatus");
                });
        }

        public void RefreshDatabaseConnectionStatus()
        {
            ExecuteInMainThreadAsync(() =>
            {
                NotifyPropertyChanged("DatabaseStatusIconSource");
                NotifyPropertyChanged("DatabaseConnectionStatus");
            });
        }

        public void ExecuteInMainThreadAsync(Action method)
        {
            Task.Factory.StartNew(method, new CancellationToken(), TaskCreationOptions.None, _taskScheduler);
        }
    }
}