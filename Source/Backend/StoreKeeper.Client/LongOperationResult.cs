using System;
using System.Collections.Generic;
using CommonBase;

namespace StoreKeeper.Client
{
    internal class LongOperationResult : ILongOperationResult
    {
        private readonly List<ObjectId> _refreshList;

        public LongOperationResult()
        {
            RefreshProductOrders = false;
            RefreshExternStorageStats = false;
            RefreshMaterialOrders = false;
            RefreshAllMaterial = false;
            RefreshAll = false;
            DataPublished = false;
            CustomAction = null;
            _refreshList = new List<ObjectId>();
        }

        #region ILongOperationResult Implementation

        public bool RefreshProductOrders { get; set; }

        public bool RefreshExternStorageStats { get; set; }

        public bool RefreshAllMaterial { get; set; }

        public bool RefreshMaterialOrders { get; set; }

        public bool RefreshAll { get; set; }

        public IEnumerable<ObjectId> MaterialRefreshList
        {
            get { return _refreshList; }
        }

        public Action CustomAction { get; set; }

        public bool DataPublished { get; set; }

        #endregion

        public void AddMaterialToRefresh(ObjectId materialId)
        {
            if (!_refreshList.Contains(materialId))
            {
                _refreshList.Add(materialId);
            }
        }
    }
}