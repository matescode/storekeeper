using CommonBase;
using CommonBase.UI.Localization;
using StoreKeeper.App.ViewModels.Common;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.Material
{
    public class MaterialViewModel : ItemViewModelBase<IMaterial>, IMaterialChangeListener
    {
        public MaterialViewModel(IMaterial material)
            : base(material)
        {
        }

        #region Properties

        public string Code
        {
            get { return Item.Code; }
        }

        public string SupplierCode
        {
            get { return Item.SupplierCode; }
        }

        public string TypeStr
        {
            get
            {
                return Item.Type.ToString().Localize(); 
            }
        }

        public string NameDescription
        {
            get { return Item.Name; }
        }

        public double StockAvailable
        {
            get { return Item.CurrentCount; }
        }

        public double CountToOrder
        {
            get { return Item.MissingInOrders; }
        }

        public double OrderedCount
        {
            get { return Item.OrderedCount; }
        }

        public int ProductCount
        {
            get { return Item.ProductCount; }
        }

        public double ExternStorageCount
        {
            get { return Item.ExternStorageCount; }
        }

        public string Price
        {
            get { return Item.Price.ToString("0.00"); }
        }

        #endregion

        public ObjectId MaterialId
        {
            get { return Item.MaterialId; }
        }

        public void Notify(string property)
        {
            NotifyPropertyChanged(property);
        }

        public void NotifyAll()
        {
            NotifyPropertyChanged("CountToOrder");
            NotifyPropertyChanged("OrderedCount");
            NotifyPropertyChanged("ProductCount");
            NotifyPropertyChanged("ExternStorageCount");
        }
    }
}