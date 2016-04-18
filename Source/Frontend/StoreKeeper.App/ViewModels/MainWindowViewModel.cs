using System;
using System.Threading.Tasks;
using CommonBase;
using CommonBase.Application;
using CommonBase.UI;
using CommonBase.UI.Localization;

using StoreKeeper.App.Searching;
using StoreKeeper.App.ViewModels.ExternStorage;
using StoreKeeper.App.ViewModels.Material;
using StoreKeeper.App.ViewModels.MaterialOrder;
using StoreKeeper.App.ViewModels.ProductOrder;
using StoreKeeper.Client;

namespace StoreKeeper.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, ICurrentStorageHolder
    {
        private readonly TaskScheduler _taskScheduler;
        private readonly IDataAccess _dataAccess;
        private MaterialListViewModel _materialsData;
        private MaterialOrderListViewModel _materialOrdersData;
        private ProductOrderListViewModel _productOrdersData;
        private ExternStorageMaterialListViewModel _externStorageMaterialsData;
        private InformationPanelViewModel _informationPanelViewModel;
        private TodayStatusViewModel _todayStatusViewModel;
        private readonly ISearchProvider[] _searchProviders;

        public MainWindowViewModel(TaskScheduler scheduler, IDataAccess dataAccess)
        {
            _taskScheduler = scheduler;
            _dataAccess = dataAccess;
            _searchProviders = new ISearchProvider[] { null, null, null, null };
            CurrentStorage = ObjectId.Empty;
        }

        #region Properties

        public MaterialListViewModel MaterialsData
        {
            get
            {
                if (_materialsData == null)
                {
                    MaterialListNotificator.Create();
                    _materialsData = new MaterialListViewModel(_dataAccess);
                    _materialsData.Load();
                    _searchProviders[1] = _materialsData;
                }
                return _materialsData;
            }
        }

        public MaterialOrderListViewModel MaterialOrdersData
        {
            get
            {
                if (_materialOrdersData == null)
                {
                    _materialOrdersData = new MaterialOrderListViewModel(_dataAccess);
                    _materialOrdersData.Load();
                    _searchProviders[2] = _materialOrdersData;
                }
                return _materialOrdersData;
            }
        }

        public ProductOrderListViewModel ProductOrdersData
        {
            get
            {
                if (_productOrdersData == null)
                {
                    _productOrdersData = new ProductOrderListViewModel(_dataAccess, ReloadProductOrders);
                    _productOrdersData.Load();
                    _searchProviders[0] = _productOrdersData;
                }
                return _productOrdersData;
            }
        }

        public ExternStorageMaterialListViewModel ExternStorageMaterialsData
        {
            get
            {
                if (_externStorageMaterialsData == null)
                {
                    _externStorageMaterialsData = new ExternStorageMaterialListViewModel(_dataAccess, this);
                    _externStorageMaterialsData.Load();
                    _searchProviders[3] = _externStorageMaterialsData;
                }
                return _externStorageMaterialsData;
            }
        }

        public InformationPanelViewModel InformationPanelModel
        {
            get { return _informationPanelViewModel ?? (_informationPanelViewModel = new InformationPanelViewModel(_taskScheduler)); }
        }

        public TodayStatusViewModel TodayStatusModel
        {
            get { return _todayStatusViewModel ?? (_todayStatusViewModel = new TodayStatusViewModel()); }
        }

        public object RetrieveSearchedItem(int tabIndex, string codePrefix)
        {
            ISearchProvider searchProvider = _searchProviders[tabIndex];
            return searchProvider.FindItem(codePrefix);
        }

        #endregion

        #region ICurrentStorageHolder Implementation

        public ObjectId CurrentStorage { get; set; }

        #endregion

        #region Public Methods

        public void Reload()
        {
            MaterialListNotificator.Clear();
            _materialsData = null;
            NotifyPropertyChanged("MaterialsData");
            _materialOrdersData = null;
            NotifyPropertyChanged("MaterialOrdersData");
            _productOrdersData = null;
            NotifyPropertyChanged("ProductOrdersData");
            _externStorageMaterialsData = null;
            NotifyPropertyChanged("ExternStorageMaterialsData");
        }

        public void ReloadProductOrders()
        {
            _productOrdersData = null;
            NotifyPropertyChanged("ProductOrdersData");
        }

        public void Reload(ILongOperationResult result)
        {
            try
            {
                if (result == null)
                {
                    return;
                }

                if (result.CustomAction != null)
                {
                    result.CustomAction();
                }

                if (result.RefreshAll)
                {
                    Reload();
                    return;
                }

                if (result.DataPublished)
                {
                    UIApplication.MessageDialogs.Info("DataSaved".Localize());
                    Reload();
                    return;
                }

                if (result.RefreshProductOrders)
                {
                    _productOrdersData = null;
                    NotifyPropertyChanged("ProductOrdersData");
                }

                if (result.RefreshMaterialOrders)
                {
                    _materialOrdersData = null;
                    NotifyPropertyChanged("MaterialOrdersData");
                }

                if (result.RefreshExternStorageStats)
                {
                    _externStorageMaterialsData = null;
                    NotifyPropertyChanged("ExternStorageMaterialsData");
                }

                if (result.RefreshAllMaterial)
                {
                    _materialsData = null;
                    NotifyPropertyChanged("MaterialsData");
                }

                if (result.MaterialRefreshList != null)
                {
                    foreach (ObjectId materialId in result.MaterialRefreshList)
                    {
                        MaterialListNotificator.Notify(materialId);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationContext.Log.Error(GetType(), ex);
            }
        }

        #endregion
    }
}