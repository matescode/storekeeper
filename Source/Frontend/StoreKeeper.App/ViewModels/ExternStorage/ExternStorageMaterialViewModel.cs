using System;
using System.Windows.Media;
using CommonBase;

using StoreKeeper.App.Printing.DeliveryNote;
using StoreKeeper.App.ViewModels.Common;
using StoreKeeper.App.ViewModels.Material;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.ExternStorage
{
    public class ExternStorageMaterialViewModel : ItemViewModelBase<IExternStorageMaterial>, IDeliveryNoteItem
    {
        private bool _isSelected;
        private bool _isSelectEnabled;

        public ExternStorageMaterialViewModel(IExternStorageMaterial externStorageMaterial)
            : base(externStorageMaterial)
        {
        }

        #region Properties

        public ObjectId StatId
        {
            get { return Item.StatId; }
        }

        public string Code
        {
            get { return Item.Code; }
        }

        public string NameDescription
        {
            get { return Item.Name; }
        }

        public double CurrentCount
        {
            get { return Item.CurrentCount; }
        }

        public string Company
        {
            get { return Item.Company; }
        }

        public double CentralStorageCount
        {
            get { return Item.CentralStorageCount; }
        }

        public double DeliverCount
        {
            get { return Item.MissingCount; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        public bool IsSelectEnabled
        {
            get { return _isSelectEnabled; }
            set
            {
                _isSelectEnabled = value;
                NotifyPropertyChanged("IsSelectEnabled");
            }
        }

        #endregion

        #region IDeliveryNoteItem Implementation

        public string SpecialCode
        {
            get { return Item.SpecialCode; }
        }

        public string CentralCode
        {
            get { return Code; }
        }

        public string Name
        {
            get { return NameDescription; }
        }

        public double AmountValue
        {
            get { return -999.99; }
        }

        public string Amount
        {
            get { return String.Format("{0} ks   ", "<NA>"); }
        }

        #endregion

        #region Public Methods

        public void RefreshCount()
        {
            NotifyPropertyChanged("CurrentCount");
            NotifyPropertyChanged("CentralStorageCount");
            MaterialListNotificator.Notify(Item.MaterialId, "StockAvailable");
            MaterialListNotificator.Notify(Item.MaterialId, "ExternStorageCount");
        }

        #endregion
    }
}