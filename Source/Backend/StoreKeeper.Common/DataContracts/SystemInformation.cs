using System;
using System.Runtime.Serialization;
using StoreKeeper.Common.DataContracts.Sync;

namespace StoreKeeper.Common.DataContracts
{
    [DataContract]
    public class SystemInformation
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}