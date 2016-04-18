using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using CommonBase.Application;
using CommonBase.Resources;
using CommonBase.UI;
using CommonBase.UI.Localization;
using CommonBase.UI.MessageDialogs;

using StoreKeeper.App.Controls;
using StoreKeeper.App.Searching;
using StoreKeeper.App.ViewModels;
using StoreKeeper.App.ViewModels.ServerAdministration;
using StoreKeeper.App.ViewModels.Storage;
using StoreKeeper.Client;

namespace StoreKeeper.App.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly TaskScheduler _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        private IStoreKeeperServiceClient _client;
        private MainWindowViewModel _viewModel;
        private SearchScrollbar[] _searchScrollbars;

        public MainWindow()
        {
            InitializeComponent();
            InitApplicationAsync(1);
        }

        private void InitApplicationAsync(int delayInSeconds)
        {
            Task.Factory.StartNew(() =>
                {
                    ILongOperationHandler handler = new LongOperationHandler(_taskScheduler);
                    try
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(delayInSeconds));
                        handler.Start("InitApp");
                        InitCommands();
                        InitInternals(_taskScheduler);
                        InitSearching();
                        if (!AppContext.Config.IsOffline && Client.DatabaseStatus == DatabaseStatus.Connected)
                        {
                            ConnectToServer();
                        }
                        handler.End();
                    }
                    catch (Exception ex)
                    {
                        handler.OperationFailed(ex.Message);
                    }

                    if (Client.DatabaseStatus == DatabaseStatus.NotConnected)
                    {
                        ExecuteInMainThreadAsync(() =>
                            {
                                UIApplication.MessageDialogs.Error("DatabaseNotConnectedError".Localize());
                                System.Windows.Application.Current.Shutdown();
                            });
                    }
                });
        }

        private void InitCommands()
        {
            Commands.ShowLogCommand.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Alt | ModifierKeys.Control));
            Commands.ShowServerAdministrationCommand.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Alt | ModifierKeys.Control));
            Commands.StartSearchCommand.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));
            Commands.CalculationCommand.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
            Commands.CalculationCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            Commands.SyncCommand.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control));
            Commands.UpdateCommand.InputGestures.Add(new KeyGesture(Key.U, ModifierKeys.Control));
        }

        private IStoreKeeperServiceClient Client
        {
            get { return _client; }
        }

        private void InitInternals(TaskScheduler scheduler)
        {
            _client = ApplicationContext.Service<IStoreKeeperServiceClient>();

            if (Client.DatabaseStatus == DatabaseStatus.NotConnected)
            {
                return;
            }

            Client.ReloadAllData(false);
            
            _viewModel = new MainWindowViewModel(_taskScheduler, Client.DataAccess);
            Client.LongOperationHandler = new LongOperationHandler(scheduler, _viewModel.Reload);
            Client.Messenger = _viewModel.InformationPanelModel;

            Task task = ExecuteInMainThreadAsync(() =>
                {
                    DataContext = _viewModel;
                    InfoPanelControl.InformationIconStack.Visibility = Visibility.Visible;
                });

            Task.WaitAll(task);
        }

        private void InitSearching()
        {
            _searchScrollbars = new[]
                {
                    new SearchScrollbar(ProductOrderList),
                    new SearchScrollbar(MaterialList),
                    new SearchScrollbar(MaterialOrderList),
                    new SearchScrollbar(ExternStorageList)
                };
        }

        private void ConnectToServer()
        {
            Task.Factory.StartNew(() =>
                {
                    int count = 1;
                    while (true)
                    {
                        try
                        {
                            ApplicationContext.Log.Info(GetType(), "Connecting to server - attempt #{0}", count);
                            if (Client.Connect())
                            {
                                ApplicationContext.Log.Info(GetType(), "Connection successfull.");
                                _viewModel.InformationPanelModel.RefreshConnectionStatus();
                                _viewModel.InformationPanelModel.RefreshDatabaseConnectionStatus();
                                break;
                            }
                            else
                            {
                                ApplicationContext.Log.Error(GetType(), "Connecting to server failed.");
                                Thread.Sleep(TimeSpan.FromMinutes(1));
                                count++;
                            }
                        }
                        catch (Exception ex)
                        {
                            ApplicationContext.Log.Error(GetType(), ex);
                            Thread.Sleep(TimeSpan.FromMinutes(1));
                            count++;
                        }
                    }
                });
        }

        private Task ExecuteInMainThreadAsync(Action method)
        {
            return Task.Factory.StartNew(method, new CancellationToken(), TaskCreationOptions.None, _taskScheduler);
        }

        #region Command Handlers

        private void ShowLogCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            LogWindow logWindow = new LogWindow();
            logWindow.Owner = this;
            logWindow.ShowDialog();
        }

        private void CanExecuteSyncCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Client.ConnectionStatus == ConnectionStatus.Connected;
        }

        private void SyncCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Client.GetCurrentAccountingDataAsync(false);
        }

        private void CanExecuteSyncAllCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Client.ConnectionStatus == ConnectionStatus.Connected;
        }

        private void SyncAllCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (UIApplication.MessageDialogs.Question("ReloadQuestion".Localize()) == QuestionResult.Positive)
            {
                Client.GetCurrentAccountingDataAsync(true);
            }
        }

        private void ApplicationExitCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void ShowAboutWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            UIApplication.ShowAboutWindow(
                new ClientApplicationInfo(),
                UIApplication.Service<IResourceProvider>().GetResource<ImageSource>("IconApplication"),
                "Eula1",
                "Copyright1");
        }

        private void CanExecuteEditStoragesCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Client.DatabaseOperationAllowed;
        }

        private void EditStoragesCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            StorageListViewModel viewModel = new StorageListViewModel(_client.DataAccess);
            viewModel.Load();

            StorageListWindow window = new StorageListWindow
                {
                    DataContext = viewModel
                };
            window.ShowDialog();
            if (viewModel.IsChanged)
            {
                _viewModel.ExternStorageMaterialsData.ReloadStorageList();
            }
        }

        private void ShowOptionsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.DataContext = new SettingsWindowViewModel(Client.DataAccess.DeliveryNoteSettings, settingsWindow.Close);
            settingsWindow.ShowDialog();
        }

        private void StartSearchCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SearchBox.Focus();
        }

        private void CanExecuteSearchCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !String.IsNullOrWhiteSpace(SearchBox.Text);
        }

        private void SearchCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            object item = _viewModel.RetrieveSearchedItem(MainTabControl.SelectedIndex, SearchBox.Text);
            if (item == null)
            {
                UIApplication.MessageDialogs.Warning("SearchNotFound".Localize());
                SearchBox.Focus();
            }
            else
            {
                SearchScrollbar searchScrollbar = _searchScrollbars[MainTabControl.SelectedIndex];
                searchScrollbar.ScrollInto(item);
                SearchBox.Text = "";
            }
        }

        private void CanExecuteShowServerAdministrationCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Client.ConnectionStatus == ConnectionStatus.Connected;
        }

        private void ShowServerAdministrationCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (Client.ConnectionStatus != ConnectionStatus.Connected)
            {
                UIApplication.MessageDialogs.Warning("CannotOpenAdministration".Localize());
                return;
            }

            ServerAdministrationViewModel viewModel = new ServerAdministrationViewModel(Client.ServerAdministration);
            ServerAdministrationWindow administrationWindow = new ServerAdministrationWindow { DataContext = viewModel };
            administrationWindow.ShowDialog();
        }

        private void CalculationCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Client.CalculationAndSaveAsync();
        }

        private void CanExecuteCalculationCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Client.ConnectionStatus == ConnectionStatus.Connected && Client.DatabaseOperationAllowed && _viewModel.InformationPanelModel.DataUpdated;
        }

        private void UpdateCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Client.ReloadAllData(true, () => _viewModel.Reload());
        }

        private void CanExecuteUpdateMethod(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !_viewModel.InformationPanelModel.DataUpdated;
        }

        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            if (UIApplication.MessageDialogs.Question("CloseAppQuestion".Localize()) == QuestionResult.Negative)
            {
                e.Cancel = true;
            }
        }

        #endregion
    }
}