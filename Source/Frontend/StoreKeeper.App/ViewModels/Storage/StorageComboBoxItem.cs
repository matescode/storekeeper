using CommonBase;

namespace StoreKeeper.App.ViewModels.Storage
{
    public class StorageComboBoxItem
    {
        private readonly ObjectId _id;
        private readonly string _name;

        public StorageComboBoxItem(ObjectId id, string name)
        {
            _id = id;
            _name = name;
        }

        public ObjectId Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        #region Overrides

        public override bool Equals(object obj)
        {
            StorageComboBoxItem item = obj as StorageComboBoxItem;
            if (item == null)
            {
                if (obj is ObjectId)
                {
                    return obj.Equals(Id);
                }
                return false;
            }
            return item.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override string ToString()
        {
            return _name;
        }

        protected bool Equals(StorageComboBoxItem other)
        {
            return Equals(_id, other._id);
        }

        #endregion 
    }
}