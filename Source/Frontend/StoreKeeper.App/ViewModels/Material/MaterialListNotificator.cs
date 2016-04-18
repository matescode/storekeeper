using System.Collections.Generic;
using CommonBase;

namespace StoreKeeper.App.ViewModels.Material
{
    internal class MaterialListNotificator
    {
        private static MaterialListNotificator _instance;

        private readonly Dictionary<ObjectId, List<IMaterialChangeListener>> _listeners;

        private MaterialListNotificator()
        {
            _listeners = new Dictionary<ObjectId, List<IMaterialChangeListener>>();
        }

        #region Properties

        private static MaterialListNotificator Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Public Methods

        public static void Create()
        {
            if (_instance != null)
            {
                return;
            }
            _instance = new MaterialListNotificator();
        }

        public static void Clear()
        {
            Instance._listeners.Clear();
        }

        public static void RegisterListener(IMaterialChangeListener listener)
        {
            if (!Instance._listeners.ContainsKey(listener.MaterialId))
            {
                Instance._listeners.Add(listener.MaterialId, new List<IMaterialChangeListener> { listener });
            }
            else
            {
                Instance._listeners[listener.MaterialId].Add(listener);
            }
        }

        public static void UnregisterListener(IMaterialChangeListener listener)
        {
            if (Instance._listeners.ContainsKey(listener.MaterialId))
            {
                Instance._listeners[listener.MaterialId].Remove(listener);
            }
        }

        public static void Notify(ObjectId materialId, string property)
        {
            List<IMaterialChangeListener> materialChangeListeners;
            if (Instance._listeners.TryGetValue(materialId, out materialChangeListeners))
            {
                foreach (IMaterialChangeListener listener in materialChangeListeners)
                {
                    listener.Notify(property);
                }
            }
        }

        public static void Notify(ObjectId materialId)
        {
            List<IMaterialChangeListener> materialChangeListeners;
            if (Instance._listeners.TryGetValue(materialId, out materialChangeListeners))
            {
                foreach (IMaterialChangeListener listener in materialChangeListeners)
                {
                    listener.NotifyAll();
                }
            }
        }

        #endregion
    }
}