using System;

namespace StoreKeeper.Common.DataContracts.Server
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string SecurityToken { get; set; }
    }
}
