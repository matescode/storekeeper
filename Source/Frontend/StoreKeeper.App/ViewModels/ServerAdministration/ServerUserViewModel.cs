using StoreKeeper.App.ViewModels.Common;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.ServerAdministration
{
    public class ServerUserViewModel : ItemViewModelBase<IServerUser>
    {
        public ServerUserViewModel(IServerUser serverUser)
            : base(serverUser)
        {
        }

        #region Properties

        public string Name
        {
            get { return Item.Name; }
            set { Item.Name = value; }
        }

        public string SecurityToken
        {
            get { return Item.SecurityToken; }
        }

        #endregion
    }
}