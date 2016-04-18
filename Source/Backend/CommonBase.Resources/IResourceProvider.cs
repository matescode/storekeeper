namespace CommonBase.Resources
{
    public interface IResourceProvider
    {
        T GetResource<T>(string resourceKey) where T : class;
    }
}