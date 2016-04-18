using System.Collections.Generic;

namespace CommonBase.Service
{
    internal class ServiceData : IServiceData
    {
        private Dictionary<string, object> _data;

        public ServiceData()
        {
            _data = new Dictionary<string, object>();
        }

        #region IServiceData Implementation

        public object this[string index]
        {
            get
            {
                return Get(index);
            }
            set
            {
                Set(index, value);
            }
        }

        public void Set(string key, object value)
        {
            _data[key] = value;
        }

        public object Get(string key)
        {
            object result;
            if (_data.TryGetValue(key, out result))
            {
                return result;
            }
            return null;
        }

        public void Remove(string key)
        {
            _data.Remove(key);
        }

        public void Clear()
        {
            _data.Clear();
        }

        #endregion
    }
}