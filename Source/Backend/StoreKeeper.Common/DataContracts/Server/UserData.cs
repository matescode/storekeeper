using System;
using System.Runtime.Serialization;

namespace StoreKeeper.Common.DataContracts.Server
{
    [DataContract]
    public class UserData
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string SecurityToken { get; set; }
    }
}