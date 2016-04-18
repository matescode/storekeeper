using System;
using System.Windows;

using CommonBase;
using StoreKeeper.App.ViewModels.Common;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.Storage
{
    public class StorageViewModel : ItemViewModelBase<IExternStorage>
    {
        public StorageViewModel(IExternStorage externStorage)
            : base(externStorage)
        {
        }

        #region Properties

        public ObjectId StorageId
        {
            get { return Item.StorageId; }
        }

        public string Name
        {
            get
            {
                return Item.Name;
            }
        }

        public string Prefix
        {
            get { return Item.Prefix; }
            set { Item.Prefix = value; }
        }

        public Visibility DeleteButtonVisibility
        {
            get { return Item.IsExtern ? Visibility.Visible : Visibility.Hidden; }
        }

        #endregion
    }
}