namespace CommonBase
{
    public interface IServiceProvider
    {
        T GetService<T>() where T : class;
    }
}