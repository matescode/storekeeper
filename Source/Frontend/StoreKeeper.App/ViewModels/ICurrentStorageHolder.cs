using CommonBase;

namespace StoreKeeper.App.ViewModels
{
    public interface ICurrentStorageHolder
    {
        ObjectId CurrentStorage { get; set; }
    }
}