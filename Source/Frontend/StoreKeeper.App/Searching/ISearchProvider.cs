namespace StoreKeeper.App.Searching
{
    public interface ISearchProvider
    {
        object FindItem(string codePrefix);
    }
}