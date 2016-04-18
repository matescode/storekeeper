using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreKeeper.Common.DataContracts.StoreKeeper
{
    public class ProductOrderCompletion : UserObject
    {
        public Guid ProductArticleOrderId { get; set; }

        public int Status { get; set; }

        [NotMapped]
        public OrderStatus OrderStatus
        {
            get { return (OrderStatus)Status; }
            set { Status = (int)value; }
        }
    }
}