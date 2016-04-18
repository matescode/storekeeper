namespace StoreKeeper.Client.Objects.Implementation
{
    internal class MaterialOrderStatus : IMaterialOrderStatus
    {
        private readonly IDataChange _dataChange;
        private readonly double _orderCount;
        private readonly double _articleOrderCount;
        
        public MaterialOrderStatus(IDataChange dataChange, double orderCount, double articleOrderCount)
        {
            _dataChange = dataChange;
            _orderCount = orderCount;
            _articleOrderCount = articleOrderCount;
        }

        #region IMaterialOrderStatus Implementation

        public AccountingOrderStatus AccountingOrderStatus
        {
            get
            {
                return _dataChange.GetOrderStatus(_orderCount, _articleOrderCount);
            }
        }

        public double AccountingOrderCount
        {
            get { return _articleOrderCount; }
        } 

        #endregion
    }
}