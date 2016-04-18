using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreKeeper.Common.DataContracts.StoreKeeper
{
    public class UserObject : ObjectBase
    {
        public string UserId { get; set; }

        [NotMapped]
        public int UserPriority
        {
            get { return !String.IsNullOrEmpty(UserId) ? 1 : 2; }
        }
    }
}