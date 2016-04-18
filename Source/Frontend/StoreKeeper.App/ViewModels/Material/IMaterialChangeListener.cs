using CommonBase;

namespace StoreKeeper.App.ViewModels.Material
{
    public interface IMaterialChangeListener
    {
        ObjectId MaterialId { get; }

        void Notify(string property);

        void NotifyAll();
    }
}