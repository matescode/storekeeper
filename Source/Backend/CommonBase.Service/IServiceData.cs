namespace CommonBase.Service
{
    public interface IServiceData
    {
        object this[string index] { get; set; }

        void Set(string key, object value);

        object Get(string key);

        void Remove(string key);

        void Clear();
    }
}